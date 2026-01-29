using Microsoft.EntityFrameworkCore;
using Chartify.Infrastructure.ChartEntries;

namespace Chartify.Infrastructure.Persistence;

public class ChartifyDbContext : DbContext
{
    public ChartifyDbContext(DbContextOptions<ChartifyDbContext> options)
        : base(options) { }

    public DbSet<ChartEntity> Charts => Set<ChartEntity>();
    public DbSet<ChartEntryEntity> ChartEntries => Set<ChartEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChartEntity>(entity =>
        {
            entity.HasKey(c => new { c.Date, c.Region });

            entity.Property(c => c.Region)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Type)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue("daily");

            entity.HasIndex(c => c.Region);
            entity.HasIndex(c => c.Date);
        });

        modelBuilder.Entity<ChartEntryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TrackSpotifyId)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.TrackName)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.ArtistNames)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasOne(e => e.Chart)
                .WithMany(c => c.Entries)
                .HasForeignKey(e => new { e.ChartDate, e.ChartRegion })
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ChartDate, e.ChartRegion });
            entity.HasIndex(e => e.TrackSpotifyId);
        });
    }
}