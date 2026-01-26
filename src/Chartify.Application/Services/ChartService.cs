using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;

namespace Chartify.Application{

    public class ChartService : IChartService
    {
        private readonly ICacheService _cache;
        private readonly IChartRepository _repo;
        public ChartService(
            ICacheService cache,
            IChartRepository repo)
        {
            _cache = cache;
            _repo = repo;
        }

        public async Task<Chart> GetGlobalTop100Async()
        {
            return await GetChartAsync("global");
        }
        public async Task<Chart> GetChartAsync(string country)
        {

            var cacheKey = $"charts:{country}:top100";

            var cached = await _cache.GetAsync<Chart>(cacheKey);
            if (cached is not null)
                return cached;


            var chart = await _repo.GetLatestAsync(country) ?? throw new Exception("Couldnt find any chart");

            await _cache.SetAsync(cacheKey, chart, TimeSpan.FromHours(24));

            return chart;
        }
    }
}