using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModelLayer.Entities;
using QuantityMeasurementRepositoryLayer.Data;
using QuantityMeasurementRepositoryLayer.Interface;

namespace QuantityMeasurementRepositoryLayer.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetUserByIdAsync(long id)
        {
            return await _context
                .Set<UserEntity>()
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            return await _context
                .Set<UserEntity>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context
                .Set<UserEntity>()
                .FirstOrDefaultAsync(u =>
                    u.Username == usernameOrEmail || u.Email == usernameOrEmail
                );
        }

        public async Task<UserEntity> CreateUserAsync(UserEntity user)
        {
            await _context.Set<UserEntity>().AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(UserEntity user)
        {
            _context.Set<UserEntity>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context
                .Set<UserEntity>()
                .AnyAsync(u => u.Username == username || u.Email == email);
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Set<UserEntity>().CountAsync();
        }

        public async Task<RefreshTokenEntity> CreateRefreshTokenAsync(
            RefreshTokenEntity refreshToken
        )
        {
            await _context.Set<RefreshTokenEntity>().AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenAsync(string token)
        {
            return await _context
                .Set<RefreshTokenEntity>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(RefreshTokenEntity refreshToken)
        {
            _context.Set<RefreshTokenEntity>().Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllUserTokensAsync(long userId, string? revokedByIp = null)
        {
            var activeTokens = await _context
                .Set<RefreshTokenEntity>()
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = revokedByIp;
            }

            await _context.SaveChangesAsync();
        }
    }
}
