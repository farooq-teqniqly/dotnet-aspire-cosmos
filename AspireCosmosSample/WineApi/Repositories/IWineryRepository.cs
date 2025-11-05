using Teqniqly.Results;
using WineApi.Entities;

namespace WineApi.Repositories
{
    /// <summary>
    /// Defines the contract for winery repository operations.
    /// </summary>
    public interface IWineryRepository
    {
        /// <summary>
        /// Creates a new winery with the specified name.
        /// </summary>
        /// <param name="name">The name of the winery to create.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A result containing the ID of the newly created winery on success, or an error on failure.</returns>
        Task<IResult<string>> CreateWineryAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves a winery by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the winery to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A result containing the winery on success, or an error on failure.</returns>
        Task<IResult<Winery>> GetWineryAsync(
            string id,
            CancellationToken cancellationToken = default
        );
    }
}
