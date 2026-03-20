using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementWebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthRepository _authRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IAuthRepository authRepository,
            IAuditLogService auditLogService,
            IConfiguration configuration,
            ILogger<AuthController> logger
        )
        {
            _authService = authService;
            _authRepository = authRepository;
            _auditLogService = auditLogService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.RegisterAsync(request, ipAddress);

            if (!result.Success)
                return BadRequest(result);

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.LoginAsync(request, ipAddress);

            if (!result.Success)
                return Unauthorized(result);

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? await GetRefreshTokenFromBody();

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(
                    new AuthResponseDto { Success = false, Message = "Refresh token is required" }
                );

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

            if (!result.Success)
                return Unauthorized(result);

            SetRefreshTokenCookie(result.RefreshToken, result.ExpiresAt!.Value.AddDays(7));
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(
                    new AuthResponseDto { Success = false, Message = "User not authenticated" }
                );

            var userId = long.Parse(userIdClaim);
            var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = await _authService.LogoutAsync(refreshToken, userId, ipAddress);

            Response.Cookies.Delete("refreshToken");
            return Ok(result);
        }

        // ==================== GOOGLE OAUTH ====================

        [HttpGet("google/login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUri = "http://localhost:5000/api/v1/Auth/google/callback";
            var clientId = _configuration["Authentication:Google:ClientId"];

            var url =
                $"https://accounts.google.com/o/oauth2/v2/auth?"
                + $"client_id={clientId}&"
                + $"redirect_uri={Uri.EscapeDataString(redirectUri)}&"
                + $"response_type=code&"
                + $"scope=openid%20email%20profile&"
                + $"access_type=offline&"
                + $"prompt=consent";

            _logger.LogInformation("Redirecting to Google: {Url}", url);
            return Redirect(url);
        }

        [HttpGet("google/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string code,
            [FromQuery] string? error
        )
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogError("Google returned error: {Error}", error);
                    return BadRequest(new { success = false, message = $"Google error: {error}" });
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest(
                        new { success = false, message = "No authorization code received" }
                    );
                }

                _logger.LogInformation("Received Google auth code, exchanging for tokens...");

                // Exchange code for tokens
                var tokenResponse = await ExchangeCodeForTokens(code);

                if (tokenResponse == null)
                {
                    return BadRequest(
                        new { success = false, message = "Failed to exchange code for tokens" }
                    );
                }

                // Get user info from Google
                var userInfo = await GetGoogleUserInfo(tokenResponse.access_token);

                if (userInfo == null || string.IsNullOrEmpty(userInfo.email))
                {
                    return BadRequest(
                        new { success = false, message = "Failed to get user info from Google" }
                    );
                }

                _logger.LogInformation("Google user: {Email}", userInfo.email);

                // Find or create user
                var user = await _authRepository.GetUserByEmailAsync(userInfo.email);

                if (user == null)
                {
                    user = new UserEntity
                    {
                        Username = userInfo.email.Split('@')[0],
                        Email = userInfo.email,
                        PasswordHash = $"GOOGLE_AUTH_{userInfo.id}",
                        FirstName = userInfo.given_name ?? "",
                        LastName = userInfo.family_name ?? "",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        Role = "User",
                    };
                    user = await _authRepository.CreateUserAsync(user);

                    // Check if this is the first user
                    var userCount = await _authRepository.GetTotalUserCountAsync();
                    if (userCount == 1)
                    {
                        user.Role = "Admin";
                        await _authRepository.UpdateUserAsync(user);
                        _logger.LogInformation("First user promoted to Admin: {Email}", user.Email);
                    }
                }

                // Generate JWT tokens
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                var (accessToken, expiresAt) = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress);

                // Return JSON with tokens
                return Ok(
                    new
                    {
                        success = true,
                        message = "Google login successful",
                        accessToken = accessToken,
                        refreshToken = refreshToken,
                        expiresAt = expiresAt,
                        user = new
                        {
                            user.Id,
                            user.Username,
                            user.Email,
                            user.FirstName,
                            user.LastName,
                            user.Role,
                        },
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google callback error");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("google/error")]
        [AllowAnonymous]
        public IActionResult GoogleError([FromQuery] string message)
        {
            return BadRequest(
                new { success = false, message = message ?? "Google authentication failed" }
            );
        }

        #region Google Token Exchange Helpers

        private async Task<GoogleTokenResponse?> ExchangeCodeForTokens(string code)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            var redirectUri = "http://localhost:5000/api/v1/Auth/google/callback";

            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", clientId!),
                    new KeyValuePair<string, string>("client_secret", clientSecret!),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                }
            );

            var response = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                content
            );
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token exchange failed: {Response}", responseContent);
                return null;
            }

            return System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(
                responseContent
            );
        }

        private async Task<GoogleUserInfo?> GetGoogleUserInfo(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await httpClient.GetAsync(
                "https://www.googleapis.com/oauth2/v3/userinfo"
            );
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get user info: {Response}", responseContent);
                return null;
            }

            return System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(responseContent);
        }

        public class GoogleTokenResponse
        {
            public string access_token { get; set; } = string.Empty;
            public string refresh_token { get; set; } = string.Empty;
            public string id_token { get; set; } = string.Empty;
            public int expires_in { get; set; }
        }

        public class GoogleUserInfo
        {
            public string id { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string given_name { get; set; } = string.Empty;
            public string family_name { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string picture { get; set; } = string.Empty;
        }

        #endregion

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = "User not authenticated" });

            var profile = await _authService.GetUserProfileAsync(long.Parse(userIdClaim));
            if (profile == null)
                return NotFound(new { Message = "User not found" });

            return Ok(profile);
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult GetAuthStatus()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var username = User.Identity?.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                Secure = false,
                SameSite = SameSiteMode.Lax,
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
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(body);
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

        private (string token, DateTime expiresAt) GenerateJwtToken(UserEntity user)
        {
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var expiresAt = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("Jwt:TokenExpiryInMinutes", 15)
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task SaveRefreshTokenAsync(long userId, string token, string? ipAddress)
        {
            var refreshToken = new RefreshTokenEntity
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int>("Jwt:RefreshTokenExpiryInDays", 7)
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
            };

            await _authRepository.CreateRefreshTokenAsync(refreshToken);
        }

        #endregion
    }
}
