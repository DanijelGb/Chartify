using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.ChartEntries;

namespace Chartify.Infrastructure.Persistence;

public class ChartifyDbContext : DbContext
{
    public ChartifyDbContext(DbContextOptions<ChartifyDbContext> options)
        : base(options) { }

    public DbSet<ChartEntity> Charts => Set<ChartEntity>();
    public DbSet<ChartEntryEntity> ChartEntries => Set<ChartEntryEntity>();
}