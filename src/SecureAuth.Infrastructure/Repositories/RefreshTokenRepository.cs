using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;
using SecureAuth.Domain.Repositories;
using SecureAuth.Infrastructure.Persistence;

namespace SecureAuth.Infrastructure.Repositories
{

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByHashWithUserAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        // 🔹 Busca todos tokens ativos de um usuário, incluindo a entidade User
        public async Task<List<RefreshToken>> GetActiveTokensWithUserAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task UpdateAsync()
        {
            // 🔥 EF já rastreia mudanças automaticamente
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllByUserAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            await _context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.RevokedAt, now)
                    .SetProperty(x => x.RevocationReason, "REVOKED_BY_SECURITY")
                );
        }
    }
}