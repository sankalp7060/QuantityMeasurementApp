using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<QuantityMeasurementEntity> QuantityMeasurements { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        // REMOVED: public DbSet<AuditLogEntity> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure QuantityMeasurementEntity
            modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MeasurementId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.OperationType).IsRequired().HasConversion<int>();
                entity.Property(e => e.FirstOperandUnit).HasMaxLength(20);
                entity.Property(e => e.FirstOperandCategory).HasMaxLength(20);
                entity.Property(e => e.SecondOperandUnit).HasMaxLength(20);
                entity.Property(e => e.SecondOperandCategory).HasMaxLength(20);
                entity.Property(e => e.TargetUnit).HasMaxLength(20);
                entity.Property(e => e.SourceOperandUnit).HasMaxLength(20);
                entity.Property(e => e.SourceOperandCategory).HasMaxLength(20);
                entity.Property(e => e.ResultUnit).HasMaxLength(20);
                entity.Property(e => e.FormattedResult).HasMaxLength(200);
                entity.Property(e => e.IsSuccessful).IsRequired();

                entity.HasIndex(e => e.MeasurementId).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.OperationType);
                entity.HasIndex(e => e.FirstOperandCategory);
                entity.HasIndex(e => e.IsSuccessful);
            });

            // Configure UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastLoginAt);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
                entity.Property(e => e.FailedLoginAttempts).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.LockoutEnd);

                entity
                    .HasMany(e => e.RefreshTokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RefreshTokenEntity
            modelBuilder.Entity<RefreshTokenEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.RevokedByIp).HasMaxLength(50);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);
                entity.Property(e => e.CreatedByIp).HasMaxLength(50);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpiresAt);
                entity.HasIndex(e => e.RevokedAt);
            });
        }
    }
}
