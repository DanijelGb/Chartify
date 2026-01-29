using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Chartify.Application{

    public class ChartService : IChartService
    {
        private readonly ICacheService _cache;
        private readonly IChartRepository _repo;
        private readonly ILogger<ChartService> _logger;
        public ChartService(
            ICacheService cache,
            IChartRepository repo,
            ILogger<ChartService> logger)
        {
            _cache = cache;
            _repo = repo;
            _logger = logger;
        }

        public async Task<Chart> GetGlobalTop100Async()
        {
            return await GetChartAsync("global", DateOnly.FromDateTime(DateTime.UtcNow));
        }
        public async Task<Chart> GetChartAsync(string country, DateOnly date)
        {

            var cacheKey = $"charts:{country}:{date}:top100";

            var cached = await _cache.GetAsync<Chart>(cacheKey);
            if (cached is not null)
            {
                _logger.LogInformation("Chart retrieved from cache with key: {CacheKey}", cacheKey);
                return cached;
            }

            try
            {
                var chart = await _repo.GetLatestAsync(country, date)
                    ?? throw new InvalidOperationException($"No chart found for country '{country}' on date {date}");

                _logger.LogInformation("Chart retrieved from repository for country '{Country}' on {Date}", country, date);

                await _cache.SetAsync(cacheKey, chart, TimeSpan.FromHours(24));

                return chart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve chart for country '{Country}' on {Date}", country, date);
                throw;
            }
        }
    }
}