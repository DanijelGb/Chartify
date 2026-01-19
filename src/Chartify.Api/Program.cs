using Serilog;
using Chartify.Application.Interfaces;
using Chartify.Infrastructure.Spotify;
using Chartify.Application;
using Chartify.Infrastructure.Cache;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.Configure<SpotifyOptions>(
    builder.Configuration.GetSection("Spotify"));

builder.Services.AddHttpClient<ISpotifyClient, SpotifyClient>();
builder.Services.Configure<SpotifyChartOptions>(
    builder.Configuration.GetSection("SpotifyCharts"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = new ConfigurationOptions
    {
        EndPoints = { "127.0.0.1:6379" },
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
        ConnectRetry = 3
    };

    return ConnectionMultiplexer.Connect(options);
});


builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddScoped<IChartSource, SpotifyChartSource>();


var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Log.Information("Hello");

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
