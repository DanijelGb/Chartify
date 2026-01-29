using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Chartify.Infrastructure.Persistence;

public class ChartifyDbContextFactory : IDesignTimeDbContextFactory<ChartifyDbContext>
{
    public ChartifyDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Chartify.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ChartifyDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));

        return new ChartifyDbContext(optionsBuilder.Options);
    }
}