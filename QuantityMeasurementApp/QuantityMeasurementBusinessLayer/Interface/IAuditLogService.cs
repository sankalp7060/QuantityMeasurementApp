using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementBusinessLayer.Interface
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string userId,
            string username,
            string action,
            string entity,
            string? details = null,
            string? ipAddress = null,
            bool isSuccess = true,
            string? errorMessage = null
        );
        Task<IEnumerable<AuditLogEntity>> GetLogsForUserAsync(
            string userId,
            DateTime? from = null,
            DateTime? to = null
        );
        Task<IEnumerable<AuditLogEntity>> GetFailedLoginsAsync(
            DateTime? from = null,
            DateTime? to = null
        );
    }
}
