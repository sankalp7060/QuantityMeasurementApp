using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Data;

namespace QuantityMeasurementBusinessLayer.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ApplicationDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(
            string userId,
            string username,
            string action,
            string entity,
            string? details = null,
            string? ipAddress = null,
            bool isSuccess = true,
            string? errorMessage = null
        )
        {
            try
            {
                var auditLog = new AuditLogEntity
                {
                    UserId = userId,
                    Username = username,
                    Action = action,
                    Entity = entity,
                    Details = details,
                    IpAddress = ipAddress ?? "unknown",
                    Timestamp = DateTime.UtcNow,
                    IsSuccess = isSuccess,
                    ErrorMessage = errorMessage,
                };

                await _context.Set<AuditLogEntity>().AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit log");
                // Don't throw - audit logging should not break the main flow
            }
        }

        public async Task<IEnumerable<AuditLogEntity>> GetLogsForUserAsync(
            string userId,
            DateTime? from = null,
            DateTime? to = null
        )
        {
            var query = _context.Set<AuditLogEntity>().Where(l => l.UserId == userId);

            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<AuditLogEntity>> GetFailedLoginsAsync(
            DateTime? from = null,
            DateTime? to = null
        )
        {
            var query = _context
                .Set<AuditLogEntity>()
                .Where(l => l.Action == "Login" && l.IsSuccess == false);

            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }
    }
}
