using QuantityMeasurement.Shared.DTOs;

namespace QuantityMeasurement.AuthService.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress = null);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task<AuthResponseDto> LogoutAsync(string? refreshToken = null, long? userId = null, string? ipAddress = null);
    Task<UserInfoDto?> GetUserProfileAsync(long userId);
}