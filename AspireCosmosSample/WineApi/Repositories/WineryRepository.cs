using System.Net;
using Microsoft.Azure.Cosmos;
using Teqniqly.Results;
using WineApi.Entities;
using WineApi.Errors;

namespace WineApi.Repositories
{
    /// <summary>
    /// Implements repository operations for wineries using Azure Cosmos DB.
    /// </summary>
    public class WineryRepository : IWineryRepository
    {
        private readonly ILogger<WineryRepository> _logger;
        private readonly Container _wineryContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WineryRepository"/> class.
        /// </summary>
        /// <param name="wineryContainer">The Cosmos DB container for wineries.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wineryContainer"/> or <paramref name="logger"/> is null.</exception>
        public WineryRepository(
            [FromKeyedServices(CosmosDbConstants.Containers.Wineries)] Container wineryContainer,
            ILogger<WineryRepository> logger
        )
        {
            ArgumentNullException.ThrowIfNull(wineryContainer);
            ArgumentNullException.ThrowIfNull(logger);

            _wineryContainer = wineryContainer;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new winery with the specified name.
        /// </summary>
        /// <param name="name">The name of the winery to create.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A result containing the ID of the newly created winery on success, or an error on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
        public async Task<IResult<string>> CreateWineryAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var existingWineryId = await GetWineryIdByNameAsync(name, cancellationToken)
                .ConfigureAwait(false);

            if (existingWineryId is not null)
            {
                return Result.Failure<string>(new WineryAlreadyExistsError(name));
            }

            var winery = new Winery
            {
                Id = $"{EntityIdPrefixes.WineryPrefix}{Guid.CreateVersion7()}",
                Name = name.Trim(),
            };

            var response = await _wineryContainer
                .UpsertItemAsync(
                    winery,
                    new PartitionKey(winery.Id),
                    cancellationToken: cancellationToken
                )
                .ConfigureAwait(false);

            _logger.LogInformation(
                "Winery created. Response from Cosmos: StatusCode={StatusCode} RU's={RUs}",
                response.StatusCode,
                response.RequestCharge
            );

            return Result.Success(response.Resource.Id);
        }

        /// <summary>
        /// Retrieves a winery by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the winery to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A result containing the winery on success, or an error on failure.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or whitespace.</exception>
        public async Task<IResult<Winery>> GetWineryAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            try
            {
                var response = await _wineryContainer
                    .ReadItemAsync<Winery>(
                        id,
                        new PartitionKey(id),
                        cancellationToken: cancellationToken
                    )
                    .ConfigureAwait(false);

                _logger.LogInformation(
                    "Winery retrieved. Response from Cosmos: StatusCode={StatusCode} RU's={RUs}",
                    response.StatusCode,
                    response.RequestCharge
                );

                return Result.Success(response.Resource);
            }
            catch (CosmosException cosmosException)
                when (cosmosException.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.Failure<Winery>(new WineryNotFoundError(id));
            }
        }

        /// <summary>
        /// Retrieves the winery ID by searching for a winery with the specified name.
        /// </summary>
        /// <param name="name">The name of the winery to search for.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The ID of the winery if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
        private async Task<string?> GetWineryIdByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var q = new QueryDefinition(
                "SELECT TOP 1 c.id FROM c WHERE LOWER(c.name) = LOWER(@name)"
            ).WithParameter("@name", name);

            using (
                var feed = _wineryContainer.GetItemQueryIterator<dynamic>(
                    q,
                    requestOptions: new QueryRequestOptions { MaxItemCount = 1 }
                )
            )
            {
                if (!feed.HasMoreResults)
                {
                    return null;
                }

                var page = await feed.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                var first = page.Resource.FirstOrDefault();

                return (string?)first?.id;
            }
        }
    }
}
