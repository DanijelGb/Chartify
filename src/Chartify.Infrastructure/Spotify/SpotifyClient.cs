using Chartify.Domain.Entities;
using Chartify.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Chartify.Infrastructure.Spotify;

public class SpotifyClient : ISpotifyClient
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyOptions _options;
    private readonly ILogger<SpotifyClient> _logger;
    private string? _accessToken;
    private DateTime _tokenExpiresAt;
    public SpotifyClient(
        HttpClient httpClient,
        ILogger<SpotifyClient> logger,
        IOptions<SpotifyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Track>> GetTopTracksAsync(string country)
    {
        _logger.LogInformation("Using spotify token to fetch tracks for {Country}", country);

        return Array.Empty<Track>();
    }
    public async Task<IReadOnlyList<Track>> GetTopTracksFromPlaylistAsync(string playlistId)
    {

        await EnsureTokenAsync();
        var url = $"https://api.spotify.com/v1/playlists/{playlistId}/tracks?limit=50";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<PlaylistTracksResponse>(stream);

        var tracks = new List<Track>();
        int rank = 1;

        foreach (var item in data!.Items)
        {
            if (item.Track == null) continue;

            tracks.Add(new Track
            {
                Id = item.Track.Id!,
                Name = item.Track.Name!,
                Artist = item.Track.Artists.First().Name!,
                Rank = rank++
            });
        }

        return Array.Empty<Track>();
    }

    public async Task EnsureTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiresAt)
            return;

        _logger.LogInformation("Requesting Spotify access token");

        var authHeader = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}")
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, "http://accounts.spotify.com/api/token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials"
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var token = await JsonSerializer.DeserializeAsync<TokenResponse>(stream);

        _accessToken = token!.AccessToken;
        _tokenExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);

        _logger.LogInformation("Spotify token acquired");
    }

    private sealed class TokenResponse
    {
        public string AccessToken { get; set; } = default;
        public int ExpiresIn { get; set; }
    }

    private sealed class PlaylistTracksResponse
    {
        public List<Item> Items { get; set; } = [];
    }

    private sealed class Item
    {
        public SpotifyTrack? Track { get; set; }
    }

    private sealed class SpotifyTrack
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<Artist> Artists { get; set; } = [];
    }

    private sealed class Artist
    {
        public string? Name { get; set; }
    }
}
