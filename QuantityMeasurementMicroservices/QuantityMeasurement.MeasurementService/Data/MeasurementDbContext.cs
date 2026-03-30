using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.MeasurementService.Models;

namespace QuantityMeasurement.MeasurementService.Data;

public class MeasurementDbContext : DbContext
{
    public MeasurementDbContext(DbContextOptions<MeasurementDbContext> options)
        : base(options) { }

    public DbSet<Measurement> Measurements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MeasurementId).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.MeasurementId).IsUnique();
            entity.Property(e => e.OperationType).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsSuccessful).IsRequired();
        });
    }
}
