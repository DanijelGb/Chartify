using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.ChartEntries;
using Chartify.Infrastructure.Persistence;

namespace Chartify.Ingestion.Scripts;

public class ImportCsv
{
    private readonly ILogger<ImportCsv> _logger;
    private readonly ChartifyDbContext _db;
    private readonly IConfiguration _config;

    public ImportCsv(ILogger<ImportCsv> logger, ChartifyDbContext db, IConfiguration config)
    {
        _logger = logger;
        _db = db;
        _config = config;
    }

    public async Task RunAsync(ChartEntity chart)
    {
        if(chart == null)
        {
            _logger.LogError("Chart entity is null. Cannot import null chart data");
            throw new ArgumentNullException(nameof(chart), "Chart entity cannot be null");
        }

        if (chart.Entries == null || chart.Entries.Count == 0)
        {
            _logger.LogWarning("Chart has no entries to import");
            return;
        }

        var maxEntries = _config.GetValue<int>("ChartifySettings:MaxEntriesPerChart");

        if (chart.Entries.Count > maxEntries)
        {
            _logger.LogError("Chart contains {EntryCount} entries but maximum allowed is {MaxEntries}", 
                chart.Entries.Count, maxEntries);
            throw new InvalidOperationException(
                $"Chart cannot have more than {maxEntries} entries. Received {chart.Entries.Count}");
        }

        try
        {
            _logger.LogInformation("Starting chart import for {ChartDate} - {ChartRegion} with {EntryCount} entries",
                chart.Date, chart.Region, chart.Entries.Count);

            _db.Charts.Add(chart);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Chart entity has been successfully imported to DB");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error occured while saving chart");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while importing chart");
            throw;
        }
    }
}
