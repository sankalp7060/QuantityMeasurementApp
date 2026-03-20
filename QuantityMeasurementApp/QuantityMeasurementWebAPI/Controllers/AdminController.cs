using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            IAuthRepository authRepository,
            IAuditLogService auditLogService,
            ILogger<AdminController> logger
        )
        {
            _context = context;
            _authRepository = authRepository;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context
                    .Set<UserEntity>()
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.CreatedAt,
                        u.LastLoginAt,
                        u.IsActive,
                        u.Role,
                        u.FailedLoginAttempts,
                        u.LockoutEnd,
                    })
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(
                    500,
                    new { Message = "An error occurred while retrieving users" }
                );
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                var user = await _context.Set<UserEntity>().FindAsync(id);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                return Ok(
                    new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.CreatedAt,
                        user.LastLoginAt,
                        user.IsActive,
                        user.Role,
                        user.FailedLoginAttempts,
                        user.LockoutEnd,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving user" });
            }
        }

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(
            long id,
            [FromBody] UpdateRoleRequest request
        )
        {
            try
            {
                var user = await _context.Set<UserEntity>().FindAsync(id);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                if (
                    string.IsNullOrEmpty(request.Role)
                    || (request.Role != "User" && request.Role != "Admin")
                )
                    return BadRequest(new { Message = "Invalid role. Must be 'User' or 'Admin'" });

                user.Role = request.Role;
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system",
                    User.Identity?.Name ?? "system",
                    "UpdateRole",
                    "User",
                    $"User {user.Username} role changed to {request.Role}",
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Ok(new { Message = "User role updated successfully", user.Role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role for ID: {UserId}", id);
                return StatusCode(
                    500,
                    new { Message = "An error occurred while updating user role" }
                );
            }
        }

        /// <summary>
        /// Activate/Deactivate user (Admin only)
        /// </summary>
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(
            long id,
            [FromBody] UpdateStatusRequest request
        )
        {
            try
            {
                var user = await _context.Set<UserEntity>().FindAsync(id);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                // Prevent admin from deactivating their own account
                if (
                    user.Id.ToString() == User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    && !request.IsActive
                )
                    return BadRequest(new { Message = "You cannot deactivate your own account" });

                user.IsActive = request.IsActive;
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system",
                    User.Identity?.Name ?? "system",
                    request.IsActive ? "ActivateUser" : "DeactivateUser",
                    "User",
                    $"User {user.Username} {(request.IsActive ? "activated" : "deactivated")}",
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Ok(
                    new
                    {
                        Message = $"User {(request.IsActive ? "activated" : "deactivated")} successfully",
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status for ID: {UserId}", id);
                return StatusCode(
                    500,
                    new { Message = "An error occurred while updating user status" }
                );
            }
        }

        /// <summary>
        /// Unlock user account (Admin only)
        /// </summary>
        [HttpPut("users/{id}/unlock")]
        public async Task<IActionResult> UnlockUser(long id)
        {
            try
            {
                var user = await _context.Set<UserEntity>().FindAsync(id);
                if (user == null)
                    return NotFound(new { Message = "User not found" });

                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system",
                    User.Identity?.Name ?? "system",
                    "UnlockUser",
                    "User",
                    $"User {user.Username} account unlocked",
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Ok(new { Message = "User account unlocked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user for ID: {UserId}", id);
                return StatusCode(500, new { Message = "An error occurred while unlocking user" });
            }
        }

        /// <summary>
        /// Get audit logs (Admin only)
        /// </summary>
        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] string? userId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to
        )
        {
            try
            {
                var query = _context.Set<AuditLogEntity>().AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(l => l.UserId == userId);
                if (from.HasValue)
                    query = query.Where(l => l.Timestamp >= from.Value);
                if (to.HasValue)
                    query = query.Where(l => l.Timestamp <= to.Value);

                var logs = await query.OrderByDescending(l => l.Timestamp).Take(100).ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs");
                return StatusCode(
                    500,
                    new { Message = "An error occurred while retrieving audit logs" }
                );
            }
        }

        /// <summary>
        /// Get user statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var totalUsers = await _context.Set<UserEntity>().CountAsync();
                var activeUsers = await _context.Set<UserEntity>().CountAsync(u => u.IsActive);
                var inactiveUsers = totalUsers - activeUsers;
                var lockedUsers = await _context
                    .Set<UserEntity>()
                    .CountAsync(u => u.LockoutEnd > DateTime.UtcNow);
                var adminUsers = await _context
                    .Set<UserEntity>()
                    .CountAsync(u => u.Role == "Admin");

                return Ok(
                    new
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        InactiveUsers = inactiveUsers,
                        LockedUsers = lockedUsers,
                        AdminUsers = adminUsers,
                        RegularUsers = totalUsers - adminUsers,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                return StatusCode(
                    500,
                    new { Message = "An error occurred while retrieving statistics" }
                );
            }
        }

        [HttpPost("make-admin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeUserAdmin(long userId)
        {
            var user = await _context.Set<UserEntity>().FindAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            user.Role = "Admin";
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system",
                User.Identity?.Name ?? "system",
                "MakeAdmin",
                "User",
                $"User {user.Username} promoted to Admin",
                HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { Message = $"User {user.Username} is now an Admin" });
        }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
