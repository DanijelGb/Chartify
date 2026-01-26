using Microsoft.Playwright;

namespace Chartify.Ingestion.Scripts;

public static class DownloadCsv
{
    public static async Task<Stream> RunAsync()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new() { Headless = false }
        );

        var context = await browser.NewContextAsync(new()
        {
            AcceptDownloads = true,
            StorageStatePath = @"C:\Users\DaciBaci\Desktop\Chartify\src\Chartify.Ingestion\spotify-auth.json"
        });

        var page = await context.NewPageAsync();

        await page.GotoAsync(
            "https://charts.spotify.com/charts/view/regional-global-daily/latest"
        );

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await page.WaitForTimeoutAsync(2000);

        for (int i = 0; i < 9; i++)
        {
            await page.Keyboard.PressAsync("Tab");
        }

        var download = await page.RunAndWaitForDownloadAsync(async () =>
        {
            await page.Keyboard.PressAsync("Enter");
        });


        await using var stream = await download.CreateReadStreamAsync();

        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;

        return memory;

    }
}
