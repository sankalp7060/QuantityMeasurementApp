using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModelLayer.Entities
{
    [Table("AuditLogs")]
    public class AuditLogEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Entity { get; set; } = string.Empty;

        public string? Details { get; set; }

        [Required]
        [MaxLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsSuccess { get; set; } = true;

        public string? ErrorMessage { get; set; }
    }
}
