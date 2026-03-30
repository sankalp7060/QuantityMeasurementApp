using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.Shared.DTOs;
using QuantityMeasurement.UserService.Data;
using QuantityMeasurement.UserService.Models;

namespace QuantityMeasurement.UserService.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(UserDbContext context, ILogger<UserController> logger)
    {
        _context = context;
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
                .Users.Select(u => new
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
            return StatusCode(500, new { Message = "Error retrieving users" });
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
            var user = await _context.Users.FindAsync(id);
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
            _logger.LogError(ex, "Error getting user {Id}", id);
            return StatusCode(500, new { Message = "Error retrieving user" });
        }
    }

    /// <summary>
    /// Update user role (Admin only)
    /// </summary>
    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(long id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (
                string.IsNullOrEmpty(request.Role)
                || (request.Role != "User" && request.Role != "Admin")
            )
                return BadRequest(new { Message = "Role must be 'User' or 'Admin'" });

            user.Role = request.Role;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User role updated successfully", user.Role });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role for user {Id}", id);
            return StatusCode(500, new { Message = "Error updating user role" });
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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            // Prevent admin from deactivating themselves
            var currentUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;
            if (currentUserId != null && long.Parse(currentUserId) == id && !request.IsActive)
                return BadRequest(new { Message = "You cannot deactivate your own account" });

            user.IsActive = request.IsActive;
            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    Message = $"User {(request.IsActive ? "activated" : "deactivated")} successfully",
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for user {Id}", id);
            return StatusCode(500, new { Message = "Error updating user status" });
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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User account unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user {Id}", id);
            return StatusCode(500, new { Message = "Error unlocking user" });
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
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var inactiveUsers = totalUsers - activeUsers;
            var lockedUsers = await _context.Users.CountAsync(u => u.LockoutEnd > DateTime.UtcNow);
            var adminUsers = await _context.Users.CountAsync(u => u.Role == "Admin");

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
            return StatusCode(500, new { Message = "Error retrieving statistics" });
        }
    }

    /// <summary>
    /// Make a user admin (Admin only)
    /// </summary>
    [HttpPost("make-admin/{userId}")]
    public async Task<IActionResult> MakeUserAdmin(long userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            user.Role = "Admin";
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"User {user.Username} is now an Admin" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making user {UserId} admin", userId);
            return StatusCode(500, new { Message = "Error updating user role" });
        }
    }

    /// <summary>
    /// Search users by username or email (Admin only)
    /// </summary>
    [HttpGet("users/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { Message = "Search query is required" });

            var users = await _context
                .Users.Where(u =>
                    u.Username.Contains(query)
                    || u.Email.Contains(query)
                    || u.FirstName.Contains(query)
                    || u.LastName.Contains(query)
                )
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive,
                    u.Role,
                })
                .Take(20)
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with query {Query}", query);
            return StatusCode(500, new { Message = "Error searching users" });
        }
    }

    /// <summary>
    /// Delete user (Admin only - soft delete)
    /// </summary>
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            // Prevent admin from deleting themselves
            var currentUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;
            if (currentUserId != null && long.Parse(currentUserId) == id)
                return BadRequest(new { Message = "You cannot delete your own account" });

            // Soft delete - deactivate instead of hard delete
            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return StatusCode(500, new { Message = "Error deleting user" });
        }
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
