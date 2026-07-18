using InfoTrack.Solicitors.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<SearchRunEntity> SearchRuns => Set<SearchRunEntity>();
    public DbSet<SearchRunLocationEntity> SearchRunLocations => Set<SearchRunLocationEntity>();
    public DbSet<SolicitorListingEntity> SolicitorListings => Set<SolicitorListingEntity>();
    public DbSet<SolicitorQualityMarkEntity> SolicitorQualityMarks => Set<SolicitorQualityMarkEntity>();

    public static readonly string[] DefaultLocationNames =
    [
        "London", "Birmingham", "Leeds", "Manchester",
        "Sheffield", "Bradford", "Liverpool", "Bristol"
    ];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationEntity>(entity =>
        {
            entity.HasIndex(l => l.Name).IsUnique();

            entity.HasData(DefaultLocationNames.Select((name, index) => new LocationEntity
            {
                Id = index + 1,
                Name = name,
                IsActive = true
            }));
        });

        modelBuilder.Entity<SearchRunEntity>()
            .HasMany(r => r.Locations)
            .WithOne(l => l.SearchRun)
            .HasForeignKey(l => l.SearchRunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SearchRunLocationEntity>()
            .HasMany(l => l.Listings)
            .WithOne(s => s.SearchRunLocation)
            .HasForeignKey(s => s.SearchRunLocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SolicitorListingEntity>(entity =>
        {
            entity.Property(s => s.Rating).HasPrecision(3, 2);

            entity.HasMany(s => s.QualityMarks)
                .WithOne(q => q.SolicitorListing)
                .HasForeignKey(q => q.SolicitorListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
