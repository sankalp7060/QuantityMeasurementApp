using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Data;

namespace QuantityMeasurementWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

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

        [HttpGet("statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
                var adminUsers = await _context.Users.CountAsync(u => u.Role == "Admin");

                return Ok(
                    new
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        InactiveUsers = totalUsers - activeUsers,
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

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(
            long id,
            [FromBody] UpdateRoleRequest request
        )
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
                    return BadRequest(new { Message = "Invalid role. Must be 'User' or 'Admin'" });

                user.Role = request.Role;
                await _context.SaveChangesAsync();

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
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}
