using CsvHelper;
using System.Globalization;
using Chartify.Infrastructure.ChartEntries;

namespace Chartify.Ingestion.Scripts;

public class ParseCsv
{
    private readonly ILogger<ParseCsv> _logger;
    private readonly IConfiguration _config;

    public ParseCsv(ILogger<ParseCsv> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public ChartEntity Parse(Stream csvStream)
    {
        try
        {
            var maxEntries = _config.GetValue<int>("ChartifySettings:MaxEntriesPerChart");
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<ChartCsvRowMap>();

            var rows = csv.GetRecords<ChartCsvRow>().Take(maxEntries).ToList();

            _logger.LogInformation("Parsed {RowCount} rows from CSV (max: {MaxEntries})", rows.Count, maxEntries);

            var chart = new ChartEntity
            {
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

            _logger.LogInformation("Chart entity created with {EntryCount} entries", chart.Entries.Count);

            return chart;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while parsing CSV");
            throw;
        }
    }
}
