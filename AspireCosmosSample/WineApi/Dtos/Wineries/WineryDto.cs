namespace WineApi.Dtos.Wineries
{
    public sealed record WineryDto(
        string Id,
        string Name,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? UpdatedAtUtc
    );
}
