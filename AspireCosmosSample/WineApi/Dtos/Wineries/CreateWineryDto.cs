namespace WineApi.Dtos.Wineries
{
    public sealed record CreateWineryDto
    {
        public required string Name { get; init; }
    }
}
