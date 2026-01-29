using Microsoft.Playwright;

namespace Chartify.Ingestion.Scripts;

public class DownloadCsv
{
    private readonly ILogger<DownloadCsv> _logger;
    private readonly IConfiguration _config;

    public DownloadCsv(
        ILogger<DownloadCsv> logger,
        IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<Stream> RunAsync()
    {
        IPlaywright playwright = null!;
        IBrowser browser = null!;
        IBrowserContext context = null!;

        var spotifySettings = _config.GetSection("ChartifySettings");
        var storageStatePath = spotifySettings["SpotifyAuthPath"];
        var chartDownloadUrl = spotifySettings["ChartDownloadUrl"];

        try
        {
            _logger.LogInformation("Starting CSV download process from Spotify Charts");

            playwright = await Playwright.CreateAsync();
            _logger.LogDebug("Playwright instance created successfully");

            browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            _logger.LogDebug("Chromium browser launched");

            context = await browser.NewContextAsync(new()
            {
                AcceptDownloads = true,
                StorageStatePath = storageStatePath
            });
            _logger.LogDebug("Browser context created with download acceptance enabled");

            var page = await context.NewPageAsync();
            _logger.LogDebug("New page created");

            _logger.LogInformation("Navigating to Spotify global charts page");
            await page.GotoAsync(chartDownloadUrl!);
            _logger.LogDebug("Page navigation completed");

            _logger.LogDebug("Waiting for DOM content to load");
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            _logger.LogDebug("Waiting additional 2 seconds for page to fully render");
            await page.WaitForTimeoutAsync(2000);

            _logger.LogDebug("Navigating to download button using Tab key presses");
            for (int i = 0; i < 9; i++)
            {
                await page.Keyboard.PressAsync("Tab");
            }
            _logger.LogDebug("Tab navigation completed");

            _logger.LogInformation("Triggering CSV download");
            var download = await page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Keyboard.PressAsync("Enter");
        });
            _logger.LogInformation("CSV file download initiated successfully");

            _logger.LogDebug("Reading downloaded file into memory stream");
            await using var stream = await download.CreateReadStreamAsync();

            var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            _logger.LogInformation("CSV file successfully loaded into memory. Stream size: {StreamSizeBytes} bytes", memory.Length);

            return memory;
        }
        catch (PlaywrightException ex)
        {
            _logger.LogError(ex, "Playwright error occurred during CSV download. This may indicate authentication or page loading issues");
            throw;
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout occurred while waiting for page elements to load. The Spotify Charts page may be unresponsive");
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while accessing Spotify Charts. Check your internet connection");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while downloading CSV from Spotify Charts");
            throw;
        }
        finally
        {
            _logger.LogDebug("Cleaning up browser resources");
            if (context != null)
                await context.CloseAsync();
            if (browser != null)
                await browser.CloseAsync();
            playwright?.Dispose();
            _logger.LogDebug("Browser resources cleaned up successfully");
        }
    }
}