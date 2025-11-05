using Teqniqly.Results;

namespace WineApi.Errors
{
    public sealed record WineryAlreadyExistsError(string Name)
        : Error($"There is already a winery named {Name}");

    public sealed record WineryNotFoundError(string Id) : Error($"Winery with id {Id} not found");
}
