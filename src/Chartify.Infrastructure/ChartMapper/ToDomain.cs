using Chartify.Domain.Entities;
using Chartify.Infrastructure.ChartEntries;

namespace Chartify.Infrastructure.Mappers;

public static class ChartMapper
{
    public static Chart ToDomain(ChartEntity entity)
    {
        return new Chart
        {
            Country = entity.Region,
            Date = entity.Date,

            Tracks = entity.Entries.Select(e => new Track
            {
                Id = e.TrackSpotifyId,
                Name = e.TrackName,
                Artist = e.ArtistNames,
                Rank = e.Rank
            }).ToList()
        };
    }
}
