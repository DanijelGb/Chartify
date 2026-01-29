using Serilog;
using Chartify.Application.Interfaces;
using Chartify.Application;
using Chartify.Infrastructure.Cache;
using StackExchange.Redis;
using Chartify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.Repository;


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

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis")!
    )
);

builder.Services.AddDbContext<ChartifyDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")
    )
);

builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddScoped<IChartRepository, ChartRepository>();


var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Log.Information("Server running..");

//app.UseHttpsRedirection();

app.MapControllers();
app.Run();
