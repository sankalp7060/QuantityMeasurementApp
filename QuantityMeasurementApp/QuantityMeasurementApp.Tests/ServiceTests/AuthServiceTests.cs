using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementApp.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _mockAuthRepository = null!;
        private IConfiguration _configuration = null!;
        private Mock<ILogger<AuthService>> _mockLogger = null!;
        private AuthService _authService = null!;

        [SetUp]
        public void Setup()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            // Create actual configuration data instead of mocking extension methods
            var configurationData = new Dictionary<string, string?>
            {
                ["Jwt:Key"] =
                    "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:TokenExpiryInMinutes"] = "60",
                ["Jwt:RefreshTokenExpiryInDays"] = "7",
            };

            // Build actual configuration
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            _authService = new AuthService(
                _mockAuthRepository.Object,
                _configuration,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task RegisterAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User",
            };

            _mockAuthRepository
                .Setup(x => x.UserExistsAsync(request.Username, request.Email))
                .ReturnsAsync(false);

            _mockAuthRepository
                .Setup(x => x.CreateUserAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync((UserEntity u) => u);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Registration successful"));
            Assert.That(result.AccessToken, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User!.Username, Is.EqualTo(request.Username));
            Assert.That(result.User.Email, Is.EqualTo(request.Email));
        }

        [Test]
        public async Task RegisterAsync_UserAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "Test123!",
            };

            _mockAuthRepository
                .Setup(x => x.UserExistsAsync(request.Username, request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(
                result.Message,
                Is.EqualTo("User with this username or email already exists")
            );
            Assert.That(result.AccessToken, Is.Null);
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "Test123!",
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
            var user = new UserEntity
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsActive = true,
                Role = "User",
            };

            _mockAuthRepository
                .Setup(x => x.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockAuthRepository
                .Setup(x => x.UpdateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepository
                .Setup(x => x.RevokeAllUserTokensAsync(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepository
                .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity r) => r);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Login successful"));
            Assert.That(result.AccessToken, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.User, Is.Not.Null);
            Assert.That(result.User!.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "WrongPassword",
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
            var user = new UserEntity
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsActive = true,
            };

            _mockAuthRepository
                .Setup(x => x.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid username/email or password"));
            Assert.That(result.AccessToken, Is.Null);
        }

        [Test]
        public async Task LoginAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "nonexistent",
                Password = "Test123!",
            };

            _mockAuthRepository
                .Setup(x => x.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid username/email or password"));
            Assert.That(result.AccessToken, Is.Null);
        }

        [Test]
        public async Task LoginAsync_InactiveUser_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "Test123!",
            };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
            var user = new UserEntity
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsActive = false,
            };

            _mockAuthRepository
                .Setup(x => x.GetUserByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(
                result.Message,
                Is.EqualTo("Account is deactivated. Please contact support.")
            );
            Assert.That(result.AccessToken, Is.Null);
        }

        [Test]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var user = new UserEntity
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                IsActive = true,
                Role = "User",
            };

            var tokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken,
                UserId = user.Id,
                User = user,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = null,
            };

            _mockAuthRepository
                .Setup(x => x.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(tokenEntity);
            _mockAuthRepository
                .Setup(x => x.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .Returns(Task.CompletedTask);
            _mockAuthRepository
                .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .ReturnsAsync((RefreshTokenEntity r) => r);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Token refreshed successfully"));
            Assert.That(result.AccessToken, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.EqualTo(refreshToken));
        }

        [Test]
        public async Task RefreshTokenAsync_ExpiredToken_ReturnsFailure()
        {
            // Arrange
            var refreshToken = "expired-token";
            var tokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired
                RevokedAt = null,
            };

            _mockAuthRepository
                .Setup(x => x.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(tokenEntity);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Refresh token has expired"));
        }

        [Test]
        public async Task RefreshTokenAsync_RevokedToken_ReturnsFailure()
        {
            // Arrange
            var refreshToken = "revoked-token";
            var tokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = DateTime.UtcNow.AddHours(-1), // Revoked
            };

            _mockAuthRepository
                .Setup(x => x.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(tokenEntity);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Refresh token has been revoked"));
        }

        [Test]
        public async Task LogoutAsync_WithRefreshToken_RevokesToken()
        {
            // Arrange
            var refreshToken = "token-to-revoke";
            var tokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken,
                UserId = 1,
                RevokedAt = null,
            };

            _mockAuthRepository
                .Setup(x => x.GetRefreshTokenAsync(refreshToken))
                .ReturnsAsync(tokenEntity);
            _mockAuthRepository
                .Setup(x => x.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(refreshToken);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Logged out successfully"));
            _mockAuthRepository.Verify(
                x => x.RevokeRefreshTokenAsync(It.IsAny<RefreshTokenEntity>()),
                Times.Once
            );
        }

        [Test]
        public async Task LogoutAsync_WithUserId_RevokesAllUserTokens()
        {
            // Arrange
            long userId = 1;
            _mockAuthRepository
                .Setup(x => x.RevokeAllUserTokensAsync(userId, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(null, userId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo("Logged out successfully"));
            _mockAuthRepository.Verify(x => x.RevokeAllUserTokensAsync(userId, null), Times.Once);
        }

        [Test]
        public async Task GetUserProfileAsync_ValidUserId_ReturnsUserInfo()
        {
            // Arrange
            long userId = 1;
            var user = new UserEntity
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = "User",
            };

            _mockAuthRepository.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _authService.GetUserProfileAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Username, Is.EqualTo(user.Username));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.Role, Is.EqualTo(user.Role));
        }

        [Test]
        public async Task GetUserProfileAsync_InvalidUserId_ReturnsNull()
        {
            // Arrange
            long userId = 999;
            _mockAuthRepository
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((UserEntity?)null);

            // Act
            var result = await _authService.GetUserProfileAsync(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void PasswordHashing_BCrypt_GeneratesValidHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var isValid = BCrypt.Net.BCrypt.Verify(password, hash);

            // Assert
            Assert.That(hash, Is.Not.Null);
            Assert.That(hash, Is.Not.Empty);
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void PasswordHashing_SamePassword_DifferentHashes()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = BCrypt.Net.BCrypt.HashPassword(password);
            var hash2 = BCrypt.Net.BCrypt.HashPassword(password);

            // Assert - BCrypt includes salt so hashes should be different
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }
    }
}
