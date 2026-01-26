using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.ChartEntries;
using Chartify.Infrastructure.Persistence;

namespace Chartify.Ingestion.Scripts;

public static class ImportCsv
{
    public static async Task RunAsync(ChartEntity chart)
    {
        var options = new DbContextOptionsBuilder<ChartifyDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=chartify;Username=postgres;Password=postgres"
            )
            .Options;

        using var db = new ChartifyDbContext(options);

        db.Charts.Add(chart);
        await db.SaveChangesAsync();
    }
}
