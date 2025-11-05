using Microsoft.Azure.Cosmos;
using WineApi.Entities;

namespace WineApi.Repositories
{
    public class WineryRepository : IWineryRepository
    {
        private readonly ILogger<WineryRepository> _logger;
        private readonly Container _wineryContainer;

        public WineryRepository(
            [FromKeyedServices("Wineries")] Container wineryContainer,
            ILogger<WineryRepository> logger
        )
        {
            ArgumentNullException.ThrowIfNull(wineryContainer);
            ArgumentNullException.ThrowIfNull(logger);

            _wineryContainer = wineryContainer;
            _logger = logger;
        }

        public async Task<string> CreateWineryAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var winery = new Winery { Id = $"ry_{Guid.CreateVersion7()}", Name = name.Trim() };

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

            return response.Resource.Id;
        }

        public Task<Winery> GetWineryAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
