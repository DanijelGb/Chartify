using Chartify.Domain.Entities;

public interface ISpotifyClient
{
    Task<IReadOnlyList<Track>> GetTopTracksAsync(string country);
}
