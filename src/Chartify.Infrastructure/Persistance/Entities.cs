namespace Chartify.Infrastructure.ChartEntries;

public class ChartEntity
{
    public DateOnly Date { get; set; }
    public string Region { get; set; } = null!;
    public string Type { get; set; } = "daily";

    public List<ChartEntryEntity> Entries { get; set; } = new();
}

public class ChartEntryEntity
{
    public Guid Id { get; set; }
    public int Rank { get; set; }
    public string TrackSpotifyId { get; set; } = null!;
    public string TrackName { get; set; } = null!;
    public string ArtistNames { get; set; } = null!;
    public long Streams { get; set; }

    public DateOnly ChartDate { get; set; }
    public string ChartRegion { get; set; } = null!;
    public ChartEntity Chart { get; set; } = null!;
}