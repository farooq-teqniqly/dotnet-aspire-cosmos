namespace WineApi.Dtos.Wineries
{
    /// <summary>
    /// Represents the data required to create a new winery.
    /// </summary>
    /// <param name="Name">The name of the winery.</param>
    public sealed record CreateWineryDto(string Name);
}
