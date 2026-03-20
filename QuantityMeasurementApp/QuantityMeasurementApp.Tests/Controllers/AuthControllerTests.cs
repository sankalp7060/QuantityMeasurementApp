using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModelLayer.DTOs;
using QuantityMeasurementRepositoryLayer.Interface;
using QuantityMeasurementWebAPI.Controllers;

namespace QuantityMeasurementApp.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService = null!;
        private Mock<IAuthRepository> _mockAuthRepository = null!;
        private Mock<IAuditLogService> _mockAuditLogService = null!;
        private IConfiguration _configuration = null!;
        private Mock<ILogger<AuthController>> _mockLogger = null!;
        private AuthController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockAuditLogService = new Mock<IAuditLogService>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            // Create actual configuration data instead of mocking
            var configurationData = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestKeyForJWTTokenGenerationThatIs32CharsLong",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:TokenExpiryInMinutes"] = "15",
                ["Jwt:RefreshTokenExpiryInDays"] = "7",
                ["Frontend:Url"] = "http://localhost:3001",
            };

            // Build actual configuration
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();

            _controller = new AuthController(
                _mockAuthService.Object,
                _mockAuthRepository.Object,
                _mockAuditLogService.Object,
                _configuration,
                _mockLogger.Object
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Test]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            var request = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
            };

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };

            _mockAuthService
                .Setup(x => x.RegisterAsync(request, "127.0.0.1"))
                .ReturnsAsync(response);

            var result = await _controller.Register(request);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            var request = new RegisterRequestDto();
            _controller.ModelState.AddModelError("Username", "Required");

            var result = await _controller.Register(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "Test123!",
            };

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };

            _mockAuthService.Setup(x => x.LoginAsync(request, "127.0.0.1")).ReturnsAsync(response);

            var result = await _controller.Login(request);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequestDto
            {
                UsernameOrEmail = "testuser",
                Password = "WrongPassword",
            };

            var response = new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username/email or password",
            };

            _mockAuthService.Setup(x => x.LoginAsync(request, "127.0.0.1")).ReturnsAsync(response);

            var result = await _controller.Login(request);

            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task RefreshToken_WithValidCookie_ReturnsOk()
        {
            var refreshToken = "valid-refresh-token";

            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c["refreshToken"]).Returns(refreshToken);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockResponseCookies = new Mock<IResponseCookies>();

            var mockResponse = new Mock<HttpResponse>();
            mockResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockContext.Setup(c => c.Response).Returns(mockResponse.Object);
            mockContext
                .Setup(c => c.Connection.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse("127.0.0.1"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object,
            };

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };

            _mockAuthService
                .Setup(x => x.RefreshTokenAsync(refreshToken, "127.0.0.1"))
                .ReturnsAsync(response);

            var result = await _controller.RefreshToken();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task RefreshToken_WithoutToken_ReturnsBadRequest()
        {
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c["refreshToken"]).Returns(string.Empty);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
            mockContext
                .Setup(c => c.Connection.RemoteIpAddress)
                .Returns(System.Net.IPAddress.Parse("127.0.0.1"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object,
            };

            var result = await _controller.RefreshToken();

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Logout_AuthenticatedUser_ReturnsOk()
        {
            var userId = "1";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            var response = new AuthResponseDto
            {
                Success = true,
                Message = "Logged out successfully",
            };

            _mockAuthService.Setup(x => x.LogoutAsync(null, 1, "127.0.0.1")).ReturnsAsync(response);

            var result = await _controller.Logout(null);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task GetProfile_AuthenticatedUser_ReturnsOk()
        {
            var userId = "1";
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            var profile = new UserInfoDto
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
            };

            _mockAuthService.Setup(x => x.GetUserProfileAsync(1)).ReturnsAsync(profile);

            var result = await _controller.GetProfile();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task GetProfile_UnauthenticatedUser_ReturnsUnauthorized()
        {
            var result = await _controller.GetProfile();

            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
        }

        [Test]
        public void GetAuthStatus_Authenticated_ReturnsTrue()
        {
            var userId = "1";
            var username = "testuser";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = principal;

            var result = _controller.GetAuthStatus();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void GetAuthStatus_Unauthenticated_ReturnsFalse()
        {
            var result = _controller.GetAuthStatus();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
