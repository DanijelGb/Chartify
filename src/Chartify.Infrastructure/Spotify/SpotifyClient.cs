using Chartify.Domain.Entities;
using Chartify.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Runtime;

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


        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);


        _logger.LogInformation(
            "Bearer header = {Auth}",
            request.Headers.Authorization?.ToString()
        );

        var response = await _httpClient.SendAsync(request);


        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Spotify error {response.StatusCode}: {body}");

        //response.EnsureSuccessStatusCode();

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

        return tracks;
    }

    public async Task EnsureTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiresAt)
            return;

        _logger.LogInformation("Requesting Spotify access token");

        var authHeader = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}")
        );

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
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
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    private sealed class PlaylistTracksResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; } = [];
    }

    private sealed class Item
    {
        [JsonPropertyName("track")]
        public SpotifyTrack? Track { get; set; }
    }

    private sealed class SpotifyTrack
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> Artists { get; set; } = [];
    }

    private sealed class Artist
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
