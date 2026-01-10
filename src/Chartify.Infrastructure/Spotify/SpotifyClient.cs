using System.Data.Common;
using Chartify.Domain.Entities;
public class SpotifyClient : ISpotifyClient
{
    public async Task<IReadOnlyList<Track>> GetTopTracksAsync(string country)
    {
        Track track = new Track(){Id="Hi", Artist="Artis", Name = "Man", Rank=5};
        return null;

    }
}
