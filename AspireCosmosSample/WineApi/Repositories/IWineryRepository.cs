using Teqniqly.Results;
using WineApi.Entities;

namespace WineApi.Repositories
{
    public interface IWineryRepository
    {
        Task<IResult<string>> CreateWineryAsync(
            string name,
            CancellationToken cancellationToken = default
        );
        Task<IResult<Winery>> GetWineryAsync(
            string id,
            CancellationToken cancellationToken = default
        );
    }
}
