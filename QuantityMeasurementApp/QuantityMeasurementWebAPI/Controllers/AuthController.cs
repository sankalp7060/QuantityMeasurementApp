using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;

namespace QuantityMeasurementWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user - PUBLIC (no auth required)
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation(
                "POST /api/v1/auth/register called for username: {Username}",
                request.Username
            );

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.RegisterAsync(request, ipAddress);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        /// <summary>
        /// Login user - PUBLIC (no auth required)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("POST /api/v1/auth/login called");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.LoginAsync(request, ipAddress);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        /// <summary>
        /// Refresh access token - PUBLIC (uses refresh token from cookie or body)
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation("POST /api/v1/auth/refresh-token called");

            var refreshToken = Request.Cookies["refreshToken"] ?? await GetRefreshTokenFromBody();

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(
                    new AuthResponseDto { Success = false, Message = "Refresh token is required" }
                );
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        /// <summary>
        /// Logout user - REQUIRES AUTHENTICATION
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request = null)
        {
            _logger.LogInformation("POST /api/v1/auth/logout called");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(
                    new AuthResponseDto { Success = false, Message = "User not authenticated" }
                );
            }

            var userId = long.Parse(userIdClaim);
            var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = await _authService.LogoutAsync(refreshToken, userId, ipAddress);

            Response.Cookies.Delete("refreshToken");
            return Ok(result);
        }

        /// <summary>
        /// Get current user profile - REQUIRES AUTHENTICATION
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("GET /api/v1/auth/profile called");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User not authenticated" });
            }

            var profile = await _authService.GetUserProfileAsync(long.Parse(userIdClaim));
            if (profile == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(profile);
        }

        /// <summary>
        /// Check if user is authenticated - PUBLIC
        /// </summary>
        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetAuthStatus()
        {
            // FIX: Check if user is authenticated via User.Identity
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var username = User.Identity?.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogDebug(
                "Auth status check - IsAuthenticated: {IsAuthenticated}, User: {User}",
                isAuthenticated,
                username
            );

            return Ok(
                new
                {
                    IsAuthenticated = isAuthenticated,
                    Username = username,
                    UserId = userId,
                    Message = isAuthenticated ? "User is logged in" : "User is not logged in",
                }
            );
        }

        #region Helper Methods

        private void SetRefreshTokenCookie(string? refreshToken, DateTime expires)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = "/",
                IsEssential = true,
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private async Task<string?> GetRefreshTokenFromBody()
        {
            try
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    var jsonDoc = JsonDocument.Parse(body);
                    if (jsonDoc.RootElement.TryGetProperty("refreshToken", out var tokenElement))
                    {
                        return tokenElement.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to extract refresh token from body");
            }
            return null;
        }

        /// <summary>
        /// Debug endpoint to see token claims - PUBLIC for debugging
        /// </summary>
        [HttpGet("debug")]
        [AllowAnonymous]
        public IActionResult DebugToken()
        {
            var headers = Request.Headers;
            var authHeader = headers["Authorization"].ToString();

            var result = new
            {
                HasAuthorizationHeader = !string.IsNullOrEmpty(authHeader),
                AuthorizationHeader = authHeader,
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                AuthenticationType = User.Identity?.AuthenticationType,
                UserName = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                Headers = headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            };

            return Ok(result);
        }

        #endregion
    }
}
