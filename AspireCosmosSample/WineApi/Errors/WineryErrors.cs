using Teqniqly.Results;

namespace WineApi.Errors
{
    /// <summary>
    /// Represents an error that occurs when attempting to create a winery that already exists.
    /// </summary>
    /// <param name="Name">The name of the winery that already exists.</param>
    public sealed record WineryAlreadyExistsError(string Name)
        : Error($"There is already a winery named {Name}");

    /// <summary>
    /// Represents an error that occurs when a winery with the specified ID is not found.
    /// </summary>
    /// <param name="Id">The unique identifier of the winery that was not found.</param>
    public sealed record WineryNotFoundError(string Id) : Error($"Winery with id {Id} not found");
}
