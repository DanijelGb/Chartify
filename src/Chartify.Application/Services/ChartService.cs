using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;

namespace Chartify.Application{

    public class ChartService : IChartService
    {
        private readonly IChartSource _chartSource;
        private readonly ICacheService _cache;
        public ChartService(IChartSource chartSource, ICacheService cache)
        {
            _cache = cache;
            _chartSource = chartSource;
        }

        public async Task<Chart> GetGlobalTop100Async()
        {
            return await GetChartAsync("GLOBAL");
        }
        public async Task<Chart> GetChartAsync(string country)
        {
            var cacheKey = $"charts:{country}:top100";

            var cached = await _cache.GetAsync<Chart>(cacheKey);
            if (cached is not null)
                return cached;

            var tracks = await _chartSource.GetTopTracksAsync(country);

            var chart = new Chart
            {
                Country = country,
                Date = DateTime.UtcNow,
                Tracks = tracks
            };

            await _cache.SetAsync(cacheKey, chart, TimeSpan.FromHours(24));
            return chart;
        }


        public async Task<Chart> GetTop100ByCountryAsync(string country)
        {
            var tracks = await _chartSource.GetTopTracksAsync(country);

            return new Chart
            {
                Country = country,
                Date = DateTime.UtcNow,
                Tracks = tracks
            };
        }
    }

}