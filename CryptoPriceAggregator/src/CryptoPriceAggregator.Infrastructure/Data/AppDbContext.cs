using CryptoPriceAggregator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoPriceAggregator.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AggregatedPrice> AggregatedPrices => Set<AggregatedPrice>();

    public DbSet<SourcePrice> SourcePrices => Set<SourcePrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AggregatedPrice>(entity =>
        {
            entity.ToTable("AggregatedPrices");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Instrument)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.TimestampUtc)
                .IsRequired();

            entity.Property(x => x.Price)
                .IsRequired();

            entity.HasIndex(x => new { x.Instrument, x.TimestampUtc })
                .IsUnique();

            entity.HasMany(x => x.SourcePrices)
                .WithOne()
                .HasForeignKey(x => x.AggregatedPriceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SourcePrice>(entity =>
        {
            entity.ToTable("SourcePrices");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SourceName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Price)
                .IsRequired();
        });
    }
}