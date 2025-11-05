namespace WineApi.Dtos.Wineries
{
    /// <summary>
    /// Represents a winery.
    /// </summary>
    /// <param name="Id">The unique identifier of the winery.</param>
    /// <param name="Name">The name of the winery.</param>
    /// <param name="CreatedAtUtc">The UTC date and time when the winery was created.</param>
    /// <param name="UpdatedAtUtc">The UTC date and time when the winery was last updated, if applicable.</param>
    public sealed record WineryDto(
        string Id,
        string Name,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? UpdatedAtUtc
    );
}
