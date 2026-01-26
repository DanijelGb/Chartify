namespace Chartify.Domain.Entities;
public class Chart
{
    public string Country { get; init; } = default;
    public DateOnly Date { get; init; }
    public List<Track> Tracks { get; init; } = [];

}    

public class Track
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Artist { get; init; } = default!;
    public int Rank { get; init; }
}
