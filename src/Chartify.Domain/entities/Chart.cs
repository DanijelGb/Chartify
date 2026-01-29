namespace Chartify.Domain.Entities;
public class Chart
{
    public string Country { get; init; } = "";
    public DateOnly Date { get; init; }
    public List<Track> Tracks { get; init; } = [];

}    

public class Track
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Artist { get; init; } = "";
    public int Rank { get; init; }
}
