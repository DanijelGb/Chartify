using Chartify.Application.Interfaces;
using Chartify.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Chartify.Infrastructure.Spotify;

public class SpotifyChartSource : IChartSource
{
    private readonly ISpotifyClient _spotifyClient;
    private readonly SpotifyChartOptions _options;

    public SpotifyChartSource(
        ISpotifyClient spotifyClient,
        IOptions<SpotifyChartOptions> options)
    {
        _spotifyClient = spotifyClient;
        _options = options.Value;
    }

    public Task<IReadOnlyList<Track>> GetTopTracksAsync(string country)
    {
        var playlistId = _options.Playlists[country];
        return _spotifyClient.GetTopTracksFromPlaylistAsync(playlistId);
    }
}
