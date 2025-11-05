using WineApi.Entities;

namespace WineApi.Repositories
{
    public interface IWineryRepository
    {
        Task<string> CreateWineryAsync(string name, CancellationToken cancellationToken = default);
        Task<Winery> GetWineryAsync(string id, CancellationToken cancellationToken = default);
    }
}
