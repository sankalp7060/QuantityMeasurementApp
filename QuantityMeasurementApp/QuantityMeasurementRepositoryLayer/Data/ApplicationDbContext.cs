using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Data
{
    /// <summary>
    /// Entity Framework Core database context with migration support
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Quantity measurements table
        /// </summary>
        public DbSet<QuantityMeasurementEntity> QuantityMeasurements { get; set; }

        /// <summary>
        /// Users table for authentication
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// Refresh tokens table for authentication
        /// </summary>
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        public DbSet<AuditLogEntity> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure QuantityMeasurementEntity
            modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
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

                // Indexes for performance
                entity
                    .HasIndex(e => e.MeasurementId)
                    .IsUnique()
                    .HasDatabaseName("IX_QuantityMeasurements_MeasurementId");

                entity
                    .HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_QuantityMeasurements_CreatedAt");

                entity
                    .HasIndex(e => e.OperationType)
                    .HasDatabaseName("IX_QuantityMeasurements_OperationType");

                entity
                    .HasIndex(e => e.FirstOperandCategory)
                    .HasDatabaseName("IX_QuantityMeasurements_FirstCategory");

                entity
                    .HasIndex(e => e.IsSuccessful)
                    .HasDatabaseName("IX_QuantityMeasurements_IsSuccessful");
            });

            // Configure UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("IX_Users_Username");

                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email");

                // PasswordHash stores the BCrypt hash which includes the salt
                // No separate Salt property needed
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastLoginAt);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");

                // New fields for account lockout
                entity.Property(e => e.FailedLoginAttempts).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.LockoutEnd);

                // Relationships
                entity
                    .HasMany(e => e.RefreshTokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure RefreshTokenEntity
            modelBuilder.Entity<RefreshTokenEntity>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Token).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.RevokedByIp).HasMaxLength(50);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);
                entity.Property(e => e.CreatedByIp).HasMaxLength(50);

                // Indexes
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
                entity.HasIndex(e => e.ExpiresAt).HasDatabaseName("IX_RefreshTokens_ExpiresAt");
                entity.HasIndex(e => e.RevokedAt).HasDatabaseName("IX_RefreshTokens_RevokedAt");
            });
            // Configure AuditLogEntity
            modelBuilder.Entity<AuditLogEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Entity).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_AuditLogs_UserId");
                entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_AuditLogs_Timestamp");
                entity.HasIndex(e => e.Action).HasDatabaseName("IX_AuditLogs_Action");
            });
        }
    }
}
