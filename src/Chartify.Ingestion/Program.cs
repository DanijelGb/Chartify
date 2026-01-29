using Chartify.Ingestion.Scripts;
using Chartify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithThreadId();
});

builder.ConfigureServices((context, services) =>
{
    services.AddDbContext<ChartifyDbContext>(options =>
        options.UseNpgsql(
            context.Configuration.GetConnectionString("Postgres")
        )
    );
    services.AddScoped<DownloadCsv>();
    services.AddScoped<ParseCsv>();
    services.AddScoped<ImportCsv>();
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var downloadCsv = scope.ServiceProvider.GetRequiredService<DownloadCsv>();
    var parseCsv = scope.ServiceProvider.GetRequiredService<ParseCsv>();
    var importCsv = scope.ServiceProvider.GetRequiredService<ImportCsv>();

    await using var csvStream = await downloadCsv.RunAsync();
    var chart = parseCsv.Parse(csvStream);
    await importCsv.RunAsync(chart);
}
