using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        private const int MAX_FAILED_ATTEMPTS = 5;
        private const int LOCKOUT_MINUTES = 15;

        public AuthService(
            IAuthRepository authRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger
        )
        {
            _authRepository = authRepository;
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
                _logger.LogInformation("=== REGISTRATION STARTED ===");
                _logger.LogInformation(
                    "Email: {Email}, Username: {Username}",
                    request.Email,
                    request.Username
                );

                // Step 1: Check if user exists
                _logger.LogInformation("Step 1: Checking if user exists");
                var userExists = await _authRepository.UserExistsAsync(
                    request.Username,
                    request.Email
                );
                if (userExists)
                {
                    _logger.LogWarning("User already exists: {Email}", request.Email);
                    return new AuthResponseDto { Success = false, Message = "User already exists" };
                }
                _logger.LogInformation("Step 1: User does not exist");

                // Step 2: Hash password
                _logger.LogInformation("Step 2: Hashing password");
                string passwordHash;
                try
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
                    _logger.LogInformation("Step 2: Password hashed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BCrypt hashing failed");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Password hashing failed: {ex.Message}",
                    };
                }

                // Step 3: Create user entity
                _logger.LogInformation("Step 3: Creating user entity");
                var user = new UserEntity
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName ?? string.Empty,
                    LastName = request.LastName ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Role = "User",
                    FailedLoginAttempts = 0,
                    LockoutEnd = null,
                };
                _logger.LogInformation("Step 3: User entity created");

                // Step 4: Save to database
                _logger.LogInformation("Step 4: Saving user to database");
                UserEntity createdUser;
                try
                {
                    createdUser = await _authRepository.CreateUserAsync(user);
                    _logger.LogInformation(
                        "Step 4: User saved successfully with ID: {UserId}",
                        createdUser.Id
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database save failed");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Database error: {ex.Message}",
                    };
                }

                // Step 5: Generate JWT token
                _logger.LogInformation("Step 5: Generating JWT token");
                var (accessToken, expiresAt) = GenerateJwtToken(createdUser);
                _logger.LogInformation("Step 5: JWT token generated");

                // Step 6: Generate refresh token
                _logger.LogInformation("Step 6: Generating refresh token");
                var refreshToken = GenerateRefreshToken();
                _logger.LogInformation("Step 6: Refresh token generated");

                // Step 7: Save refresh token
                _logger.LogInformation("Step 7: Saving refresh token");
                await SaveRefreshTokenAsync(createdUser.Id, refreshToken, ipAddress);
                _logger.LogInformation("Step 7: Refresh token saved");

                _logger.LogInformation("=== REGISTRATION COMPLETED SUCCESSFULLY ===");

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
                _logger.LogError(
                    ex,
                    "Registration error - Full exception: {ExceptionType} - {Message}",
                    ex.GetType().Name,
                    ex.Message
                );
                if (ex.InnerException != null)
                {
                    _logger.LogError(
                        ex.InnerException,
                        "Inner exception: {Message}",
                        ex.InnerException.Message
                    );
                }
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}",
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request,
            string? ipAddress = null
        )
        {
            try
            {
                var user = await _authRepository.GetUserByUsernameOrEmailAsync(
                    request.UsernameOrEmail
                );

                if (user == null)
                {
                    await Task.Delay(500);
                    return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                    return new AuthResponseDto { Success = false, Message = "Account locked" };

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash
                );

                if (!isValidPassword)
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts >= MAX_FAILED_ATTEMPTS)
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);

                    await _authRepository.UpdateUserAsync(user);
                    await Task.Delay(500);
                    return new AuthResponseDto { Success = false, Message = "Invalid credentials" };
                }

                if (!user.IsActive)
                    return new AuthResponseDto { Success = false, Message = "Account deactivated" };

                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                user.LastLoginAt = DateTime.UtcNow;
                await _authRepository.UpdateUserAsync(user);

                var (accessToken, expiresAt) = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                await _authRepository.RevokeAllUserTokensAsync(user.Id, ipAddress);
                await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress);

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
                var tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
                if (tokenEntity == null)
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                    };

                if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Refresh token expired",
                    };

                if (tokenEntity.RevokedAt != null)
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Refresh token revoked",
                    };

                var user = tokenEntity.User;
                if (user == null || !user.IsActive)
                    return new AuthResponseDto { Success = false, Message = "User not found" };

                var (newAccessToken, expiresAt) = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                tokenEntity.RevokedAt = DateTime.UtcNow;
                tokenEntity.RevokedByIp = ipAddress;
                tokenEntity.ReplacedByToken = newRefreshToken;
                await _authRepository.RevokeRefreshTokenAsync(tokenEntity);

                await SaveRefreshTokenAsync(user.Id, newRefreshToken, ipAddress);

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
                    var tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);
                    if (tokenEntity != null && tokenEntity.RevokedAt == null)
                    {
                        tokenEntity.RevokedAt = DateTime.UtcNow;
                        tokenEntity.RevokedByIp = ipAddress;
                        await _authRepository.RevokeRefreshTokenAsync(tokenEntity);
                    }
                }
                else if (userId.HasValue)
                {
                    await _authRepository.RevokeAllUserTokensAsync(userId.Value, ipAddress);
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
            var user = await _authRepository.GetUserByIdAsync(userId);
            return user == null ? null : MapToUserInfo(user);
        }

        #region Helper Methods

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

        private UserInfoDto MapToUserInfo(UserEntity user)
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

        #endregion
    }
}
