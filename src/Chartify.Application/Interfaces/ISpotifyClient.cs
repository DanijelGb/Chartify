using Chartify.Domain.Entities;

namespace Chartify.Application.Interfaces;

public interface ISpotifyClient
{
    Task<IReadOnlyList<Track>> GetTopTracksAsync(string country);
}
