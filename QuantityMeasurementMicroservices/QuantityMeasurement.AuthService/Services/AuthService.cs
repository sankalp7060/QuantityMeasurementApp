using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net; 
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurement.AuthService.Models;
using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    private const int MAX_FAILED_ATTEMPTS = 5;
    private const int LOCKOUT_MINUTES = 15;

    public AuthService(
        IAuthRepository repository,
        IConfiguration configuration,
        ILogger<AuthService> logger
    )
    {
        _repository = repository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        string? ipAddress = null
    )
    {
        try
        {
            var userExists = await _repository.UserExistsAsync(request.Username, request.Email);
            if (userExists)
                return new AuthResponseDto { Success = false, Message = "User already exists" };

            // BCrypt password hashing - use BCrypt.Net.BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Role = "User",
            };

            var createdUser = await _repository.CreateUserAsync(user);
            var (accessToken, expiresAt) = GenerateJwtToken(createdUser);
            var refreshToken = GenerateRefreshToken();

            await _repository.CreateRefreshTokenAsync(
                new RefreshToken
                {
                    UserId = createdUser.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedByIp = ipAddress,
                }
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserInfo(createdUser),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return new AuthResponseDto { Success = false, Message = "Registration failed" };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null)
    {
        try
        {
            var user = await _repository.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                return new AuthResponseDto { Success = false, Message = "Account locked" };

            // BCrypt password verification - use BCrypt.Net.BCrypt
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= MAX_FAILED_ATTEMPTS)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);
                await _repository.UpdateUserAsync(user);
                return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
            }

            if (!user.IsActive)
                return new AuthResponseDto { Success = false, Message = "Account deactivated" };

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _repository.UpdateUserAsync(user);

            var (accessToken, expiresAt) = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            await _repository.RevokeAllUserTokensAsync(user.Id, ipAddress);
            await _repository.CreateRefreshTokenAsync(
                new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedByIp = ipAddress,
                }
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserInfo(user),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return new AuthResponseDto { Success = false, Message = "Login failed" };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress = null
    )
    {
        try
        {
            var tokenEntity = await _repository.GetRefreshTokenAsync(refreshToken);
            if (tokenEntity == null)
                return new AuthResponseDto { Success = false, Message = "Invalid refresh token" };

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                return new AuthResponseDto { Success = false, Message = "Refresh token expired" };

            if (tokenEntity.RevokedAt != null)
                return new AuthResponseDto { Success = false, Message = "Refresh token revoked" };

            var user = tokenEntity.User;
            if (user == null || !user.IsActive)
                return new AuthResponseDto { Success = false, Message = "User not found" };

            var (newAccessToken, expiresAt) = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            tokenEntity.RevokedAt = DateTime.UtcNow;
            tokenEntity.RevokedByIp = ipAddress;
            tokenEntity.ReplacedByToken = newRefreshToken;
            await _repository.UpdateRefreshTokenAsync(tokenEntity);

            await _repository.CreateRefreshTokenAsync(
                new RefreshToken
                {
                    UserId = user.Id,
                    Token = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedByIp = ipAddress,
                }
            );

            return new AuthResponseDto
            {
                Success = true,
                Message = "Token refreshed",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                User = MapToUserInfo(user),
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh token error");
            return new AuthResponseDto { Success = false, Message = "Token refresh failed" };
        }
    }

    public async Task<AuthResponseDto> LogoutAsync(
        string? refreshToken = null,
        long? userId = null,
        string? ipAddress = null
    )
    {
        try
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var tokenEntity = await _repository.GetRefreshTokenAsync(refreshToken);
                if (tokenEntity != null && tokenEntity.RevokedAt == null)
                {
                    tokenEntity.RevokedAt = DateTime.UtcNow;
                    tokenEntity.RevokedByIp = ipAddress;
                    await _repository.UpdateRefreshTokenAsync(tokenEntity);
                }
            }
            else if (userId.HasValue)
            {
                await _repository.RevokeAllUserTokensAsync(userId.Value, ipAddress);
            }

            return new AuthResponseDto { Success = true, Message = "Logged out" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            return new AuthResponseDto { Success = false, Message = "Logout failed" };
        }
    }

    public async Task<UserInfoDto?> GetUserProfileAsync(long userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        return user == null ? null : MapToUserInfo(user);
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(User user)
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

        var expiresAt = DateTime.UtcNow.AddMinutes(15);
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

    private UserInfoDto MapToUserInfo(User user)
    {
        return new UserInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
        };
    }
}
