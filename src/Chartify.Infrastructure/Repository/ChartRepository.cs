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

    public async Task<Chart?> GetLatestAsync(string location)
    {
        var entity = await _db.Charts
            .Include(c => c.Entries)
            .OrderByDescending(c => c.Date)
            .FirstOrDefaultAsync(c => c.Region == location);

        return entity == null ? null : ChartMapper.ToDomain(entity);
    }
}
