namespace Chartify.Domain.Entities
{
    public class Track
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Artist { get; init; } = default!;
        public int Rank { get; init; }
    }
}
