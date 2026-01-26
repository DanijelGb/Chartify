using CsvHelper.Configuration;

namespace Chartify.Ingestion.Scripts;

public sealed class ChartCsvRowMap : ClassMap<ChartCsvRow>
{
    public ChartCsvRowMap()
    {
        Map(m => m.Rank).Name("rank");
        Map(m => m.Uri).Name("uri");
        Map(m => m.Artist_Names).Name("artist_names");
        Map(m => m.Track_Name).Name("track_name");
        Map(m => m.Streams).Name("streams");
    }
}

