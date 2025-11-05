namespace WineApi.Dtos.Wineries
{
    /// <summary>
    /// Represents the response after creating a new winery.
    /// </summary>
    /// <param name="Id">The unique identifier of the newly created winery.</param>
    public sealed record CreateWineryResponseDto(string Id);
}
