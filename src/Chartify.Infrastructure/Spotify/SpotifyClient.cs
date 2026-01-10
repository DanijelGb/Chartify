using Chartify.Domain.Entities;
using Chartify.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chartify.Infrastructure.Spotify;

public class SpotifyClient : ISpotifyClient
{
    private readonly ILogger<SpotifyClient> _logger;
    public SpotifyClient(ILogger<SpotifyClient> logger)
    {
        _logger = logger;
    }
    public async Task<IReadOnlyList<Track>> GetTopTracksAsync(string country)
    {
        _logger.LogInformation("Fetching top tracks from spotify for {Country}", country);


        return new List<Track>{
            new() { Id = "1", Name = "Man", Artist = "Artis", Rank = 5 },
            new() { Id = "2", Name = "Damn", Artist = "Adele", Rank = 3 }
        };

    }
}
