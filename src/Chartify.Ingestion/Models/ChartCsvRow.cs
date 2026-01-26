public class ChartCsvRow
{
    public int Rank { get; set; }
    public string Uri { get; set; } = null!;          // spotify:track:ID
    public string Track_Name { get; set; } = null!;
    public string Artist_Names { get; set; } = null!;
    public long Streams { get; set; }
}
