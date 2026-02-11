using Chartify.Ingestion.Scripts;
using Chartify.Ingestion.Jobs;
using Chartify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;
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

    services.AddQuartz(q =>
    {
        var jobKey = new JobKey("DownloadChartJob");

        q.AddJob<DownloadChartJob>(opts =>
            opts.WithIdentity(jobKey)
                .WithDescription("Daily chart ingestion from Spotify")
        );

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("DailyChartTrigger")
            .WithDescription("Daily trigger for chart ingestion")
            .WithCronSchedule("0 0 1 * * ?") // 1 AM every day
            .StartNow()
        );
    });

    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
});

var host = builder.Build();
await host.RunAsync();

