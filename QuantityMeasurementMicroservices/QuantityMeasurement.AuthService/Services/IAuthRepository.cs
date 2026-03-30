using QuantityMeasurement.AuthService.Models;

namespace QuantityMeasurement.AuthService.Services;

public interface IAuthRepository
{
    Task<User?> GetUserByIdAsync(long id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<bool> UserExistsAsync(string username, string email);
    
    Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(long userId, string? revokedByIp = null);
}