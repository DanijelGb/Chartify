namespace Chartify.Domain.Entities
{
    public class Chart
    {
        public string Country { get; init; } = default;
        public DateTime date { get; init; }
        public IReadOnlyList<Track> Tracks{ get; init; } = [];

    }    

}