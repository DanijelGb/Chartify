using Chartify.Infrastructure.Persistence;
using Chartify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.Mappers;
using Chartify.Application.Interfaces;

namespace Chartify.Infrastructure.Repository;

public class ChartRepository : IChartRepository
{
    private readonly ChartifyDbContext _db;

    public ChartRepository(ChartifyDbContext db)
    {
        _db = db;
    }

    public async Task<Chart?> GetLatestAsync(string country, DateOnly date)
    {
        var entity = await _db.Charts
            .Where(c => c.Region == country && c.Date == date)
            .OrderByDescending(c => c.Date)
            .Include(c => c.Entries.OrderBy(e => e.Rank))
            .FirstOrDefaultAsync();

        return entity == null ? null : ChartMapper.ToDomain(entity);
    }
}
