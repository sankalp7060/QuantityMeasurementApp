using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.AuthService.Services;
using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.AuthService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IConfiguration configuration,
        ILogger<AuthController> logger
    )
    {
        _authService = authService;
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
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest(new { Success = false, Message = "Refresh token is required" });

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);

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
            return Unauthorized(new { Success = false, Message = "User not authenticated" });

        var userId = long.Parse(userIdClaim);
        var refreshToken = request?.RefreshToken;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

        var result = await _authService.LogoutAsync(refreshToken, userId, ipAddress);

        Response.Cookies.Delete("refreshToken");
        return Ok(result);
    }

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

    // ==================== GOOGLE OAUTH ====================

    [HttpGet("google/login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        var clientId = "";
        var redirectUri = "http://localhost:5001/api/v1/Auth/google/callback";

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
                return Redirect($"http://localhost:3000/login?error={Uri.EscapeDataString(error)}");
            }

            if (string.IsNullOrEmpty(code))
            {
                return Redirect("http://localhost:3000/login?error=No authorization code");
            }

            _logger.LogInformation("Received Google auth code, exchanging for tokens...");

            // Exchange code for tokens
            var tokenResponse = await ExchangeCodeForTokens(code);
            if (tokenResponse == null)
            {
                return Redirect("http://localhost:3000/login?error=Failed to exchange code");
            }

            // Get user info from Google
            var userInfo = await GetGoogleUserInfo(tokenResponse.access_token);
            if (userInfo == null || string.IsNullOrEmpty(userInfo.email))
            {
                return Redirect("http://localhost:3000/login?error=Failed to get user info");
            }

            _logger.LogInformation("Google user: {Email}", userInfo.email);

            // Register or login user via auth service
            var registerRequest = new RegisterRequestDto
            {
                Username = userInfo.email.Split('@')[0],
                Email = userInfo.email,
                Password = Guid.NewGuid().ToString(),
                ConfirmPassword = Guid.NewGuid().ToString(),
                FirstName = userInfo.given_name ?? "",
                LastName = userInfo.family_name ?? "",
            };

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = await _authService.RegisterAsync(registerRequest, ipAddress);

            if (!result.Success && result.Message != "User already exists")
            {
                return Redirect("http://localhost:3000/login?error=Failed to create user");
            }

            // If user already exists, login
            if (result.Message == "User already exists")
            {
                var loginRequest = new LoginRequestDto
                {
                    UsernameOrEmail = userInfo.email,
                    Password = registerRequest.Password,
                };
                var loginResult = await _authService.LoginAsync(loginRequest, ipAddress);
                if (loginResult.Success)
                {
                    var userJson = Uri.EscapeDataString(
                        System.Text.Json.JsonSerializer.Serialize(loginResult.User)
                    );
                    return Redirect(
                        $"http://localhost:3000/auth/callback?accessToken={loginResult.AccessToken}&refreshToken={loginResult.RefreshToken}&user={userJson}"
                    );
                }
                return Redirect("http://localhost:3000/login?error=Login failed");
            }

            // New user registered
            var userJsonNew = Uri.EscapeDataString(
                System.Text.Json.JsonSerializer.Serialize(result.User)
            );
            return Redirect(
                $"http://localhost:3000/auth/callback?accessToken={result.AccessToken}&refreshToken={result.RefreshToken}&user={userJsonNew}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google callback error");
            return Redirect("http://localhost:3000/login?error=Internal server error");
        }
    }

    #region Google Token Exchange Helpers

    private async Task<GoogleTokenResponse?> ExchangeCodeForTokens(string code)
    {
        var clientId = "";
        var clientSecret = "";
        var redirectUri = "http://localhost:5001/api/v1/Auth/google/callback";

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(
            new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            }
        );

        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Token exchange failed: {Response}", responseContent);
            return null;
        }

        return System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent);
    }

    private async Task<GoogleUserInfo?> GetGoogleUserInfo(string accessToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
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
}
