namespace WineApi.Entities
{
    public sealed class Winery
    {
        public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;
        public required string Id { get; init; }
        public required string Name { get; init; }
        public DateTimeOffset? UpdatedAtUtc { get; set; }
    }
}
