using CsvHelper;
using System.Globalization;
using Chartify.Infrastructure.ChartEntries;

namespace Chartify.Ingestion.Scripts;

public static class ParseCsv
{
    public static ChartEntity Parse(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ChartCsvRowMap>(); // ✅ ADD THIS

        var rows = csv.GetRecords<ChartCsvRow>().ToList();

        var chart = new ChartEntity
        {
            Id = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Region = "global",
            Type = "daily"
        };

        chart.Entries = rows.Select(r => new ChartEntryEntity
        {
            Id = Guid.NewGuid(),
            Rank = r.Rank,
            TrackSpotifyId = r.Uri.Split(':').Last(),
            TrackName = r.Track_Name,
            ArtistNames = r.Artist_Names,
            Streams = r.Streams
        }).ToList();

        return chart;
    }
}
