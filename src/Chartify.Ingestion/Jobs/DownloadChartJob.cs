using Quartz;
using Chartify.Ingestion.Scripts;

namespace Chartify.Ingestion.Jobs;

public class DownloadChartJob : IJob
{
    private readonly DownloadCsv _downloadCsv;
    private readonly ParseCsv _parseCsv;
    private readonly ImportCsv _importCsv;
    private readonly ILogger<DownloadChartJob> _logger;

    public DownloadChartJob(
        DownloadCsv downloadCsv,
        ParseCsv parseCsv,
        ImportCsv importCsv,
        ILogger<DownloadChartJob> logger)
    {
        _downloadCsv = downloadCsv;
        _parseCsv = parseCsv;
        _importCsv = importCsv;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting scheduled daily chart ingestion job at {StartTime}", DateTime.UtcNow);

            // Step 1: Download CSV
            _logger.LogInformation("Step 1: Downloading CSV from Spotify Charts");
            await using var csvStream = await _downloadCsv.RunAsync();

            // Step 2: Parse CSV
            _logger.LogInformation("Step 2: Parsing CSV data");
            var chartData = _parseCsv.Parse(csvStream);

            // Step 3: Import to database
            _logger.LogInformation("Step 3: Importing chart data to database");
            await _importCsv.RunAsync(chartData);

            _logger.LogInformation("Daily chart ingestion job completed successfully at {EndTime}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Daily chart ingestion job failed");
            throw;
        }
    }
}
