using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementBusinessLayer.Services
{
    /// <summary>
    /// Service implementation for authentication operations
    /// Uses BCrypt for password hashing which automatically handles salting
    /// Implements account lockout and secure token management
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        // Maximum number of failed login attempts before lockout
        private const int MAX_FAILED_ATTEMPTS = 5;

        // Lockout duration in minutes
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

        /// <summary>
        /// Registers a new user with secure password hashing
        /// BCrypt automatically generates and embeds a unique salt for each password
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request,
            string? ipAddress = null
        )
        {
            try
            {
                // Check if user already exists
                var userExists = await _authRepository.UserExistsAsync(
                    request.Username,
                    request.Email
                );
                if (userExists)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User with this username or email already exists",
                    };
                }

                // Hash the password using BCrypt with work factor 12
                // BCrypt automatically:
                // 1. Generates a cryptographically secure random salt (16 bytes)
                // 2. Combines salt with password
                // 3. Performs 2^12 iterations of the Blowfish cipher
                // 4. Returns a single string containing: version + work factor + salt + hash
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Password,
                    workFactor: 12
                );

                // Create new user entity
                var user = new UserEntity
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash, // BCrypt hash already contains the salt
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Role = "User",
                    FailedLoginAttempts = 0,
                    LockoutEnd = null,
                };

                // Save user to database
                var createdUser = await _authRepository.CreateUserAsync(user);

                // Generate JWT tokens for immediate login after registration
                var (accessToken, expiresAt) = GenerateJwtToken(createdUser);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                await SaveRefreshTokenAsync(createdUser.Id, refreshToken, ipAddress);

                _logger.LogInformation(
                    "User registered successfully: {Username}",
                    request.Username
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
                _logger.LogError(
                    ex,
                    "Error during registration for user: {Username}",
                    request.Username
                );
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Registration failed: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Authenticates a user with secure password verification
        /// Includes account lockout protection against brute force attacks
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request,
            string? ipAddress = null
        )
        {
            try
            {
                // Find user by username or email
                var user = await _authRepository.GetUserByUsernameOrEmailAsync(
                    request.UsernameOrEmail
                );

                // User not found - use same message as wrong password to prevent user enumeration
                if (user == null)
                {
                    _logger.LogWarning(
                        "Login failed - user not found: {UsernameOrEmail}",
                        request.UsernameOrEmail
                    );

                    // Add a small delay to prevent timing attacks
                    await Task.Delay(500);

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username/email or password",
                    };
                }

                // Check if account is locked
                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                {
                    _logger.LogWarning(
                        "Login failed - account locked for user: {Username}",
                        user.Username
                    );
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message =
                            $"Account is locked. Please try again after {user.LockoutEnd.Value.ToLocalTime()}.",
                    };
                }

                // Verify password using BCrypt
                // BCrypt automatically extracts the salt from the stored hash
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash
                );

                if (!isValidPassword)
                {
                    // Increment failed login attempts
                    user.FailedLoginAttempts++;

                    // Lock account if too many failed attempts
                    if (user.FailedLoginAttempts >= MAX_FAILED_ATTEMPTS)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES);
                        _logger.LogWarning(
                            "Account locked for user: {Username} due to {Attempts} failed attempts",
                            user.Username,
                            user.FailedLoginAttempts
                        );
                    }

                    await _authRepository.UpdateUserAsync(user);

                    _logger.LogWarning(
                        "Login failed - invalid password for user: {Username}",
                        user.Username
                    );

                    // Add a small delay to prevent timing attacks
                    await Task.Delay(500);

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username/email or password",
                    };
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is deactivated. Please contact support.",
                    };
                }

                // Reset failed login attempts on successful login
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                user.LastLoginAt = DateTime.UtcNow;

                await _authRepository.UpdateUserAsync(user);

                // Generate new tokens
                var (accessToken, expiresAt) = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                // Revoke all previous refresh tokens (token rotation for security)
                await _authRepository.RevokeAllUserTokensAsync(user.Id, ipAddress);

                // Save new refresh token
                await SaveRefreshTokenAsync(user.Id, refreshToken, ipAddress);

                _logger.LogInformation("User logged in successfully: {Username}", user.Username);

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
                _logger.LogError(
                    ex,
                    "Error during login for: {UsernameOrEmail}",
                    request.UsernameOrEmail
                );
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Login failed: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token
        /// Implements token rotation for enhanced security
        /// </summary>
        public async Task<AuthResponseDto> RefreshTokenAsync(
            string refreshToken,
            string? ipAddress = null
        )
        {
            try
            {
                var tokenEntity = await _authRepository.GetRefreshTokenAsync(refreshToken);

                if (tokenEntity == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                    };
                }

                // Check if token is expired
                if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Refresh token has expired",
                    };
                }

                // Check if token is revoked
                if (tokenEntity.RevokedAt != null)
                {
                    _logger.LogWarning(
                        "Attempt to use revoked token for user {UserId}",
                        tokenEntity.UserId
                    );
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Refresh token has been revoked",
                    };
                }

                var user = tokenEntity.User;
                if (user == null || !user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found or inactive",
                    };
                }

                // Generate new tokens (token rotation)
                var (newAccessToken, expiresAt) = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Revoke current refresh token
                tokenEntity.RevokedAt = DateTime.UtcNow;
                tokenEntity.RevokedByIp = ipAddress;
                tokenEntity.ReplacedByToken = newRefreshToken;
                await _authRepository.RevokeRefreshTokenAsync(tokenEntity);

                // Save new refresh token
                await SaveRefreshTokenAsync(user.Id, newRefreshToken, ipAddress);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = expiresAt,
                    User = MapToUserInfo(user),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Token refresh failed: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Logs out a user by revoking their refresh tokens
        /// </summary>
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
                    // Revoke specific refresh token
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
                    // Revoke all tokens for user
                    await _authRepository.RevokeAllUserTokensAsync(userId.Value, ipAddress);
                }

                return new AuthResponseDto { Success = true, Message = "Logged out successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Logout failed: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Gets user profile information
        /// </summary>
        public async Task<UserInfoDto?> GetUserProfileAsync(long userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            return user == null ? null : MapToUserInfo(user);
        }

        #region Helper Methods

        /// <summary>
        /// Generates a JWT token for authenticated user
        /// Uses secure signing key from configuration
        /// </summary>
        private (string token, DateTime expiresAt) GenerateJwtToken(UserEntity user)
        {
            // Get JWT key from configuration - in production this should be from environment variables
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
            ); // Short-lived tokens (15 minutes)

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }

        /// <summary>
        /// Generates a cryptographically secure random refresh token
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Saves a refresh token to the database
        /// </summary>
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

        /// <summary>
        /// Maps a UserEntity to UserInfoDto
        /// </summary>
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
