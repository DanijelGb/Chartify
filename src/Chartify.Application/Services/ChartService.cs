using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;

namespace Chartify.Application{

    public class ChartService : IChartService
    {
        private readonly ISpotifyClient _spotifyClient;

        public ChartService(ISpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
        }

        public async Task<Chart> GetGlobalTop100Async()
        {
            var tracks = await _spotifyClient.GetTopTracksAsync("GLOBAL");

            return new Chart
            {
                Country = "GLOBAL",
                date = DateTime.UtcNow,
                Tracks = tracks
            };
        }


        public async Task<Chart> GetTop100ByCountryAsync(string Country)
        {
            return null;
        }
    }

}