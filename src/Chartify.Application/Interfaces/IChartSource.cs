using Chartify.Domain.Entities;

namespace Chartify.Application.Interfaces;

public interface IChartSource
{
    Task<IReadOnlyList<Track>> GetTopTracksAsync(string country);
}