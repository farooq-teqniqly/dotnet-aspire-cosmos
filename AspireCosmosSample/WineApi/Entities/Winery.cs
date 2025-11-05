namespace WineApi.Entities
{
    /// <summary>
    /// Represents a winery entity.
    /// </summary>
    public sealed class Winery
    {
        /// <summary>
        /// Gets the UTC date and time when the winery was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or initializes the unique identifier of the winery.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Gets or initializes the name of the winery.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets the UTC date and time when the winery was last updated.
        /// </summary>
        public DateTimeOffset? UpdatedAtUtc { get; set; }
    }
}
