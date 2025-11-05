using Teqniqly.Results;

namespace WineApi.Errors
{
    public sealed record WineryAlreadyExistsError(string Name)
        : Error($"The winery named {Name} already exists");
}
