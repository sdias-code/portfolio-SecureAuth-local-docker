using SecureAuth.Domain.Entities;

namespace SecureAuth.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken); 
        Task<RefreshToken?> GetByHashWithUserAsync(string tokenHash);
        Task<List<RefreshToken>> GetActiveTokensWithUserAsync(Guid userId);
        Task RevokeAllByUserAsync(Guid userId);
        Task UpdateAsync();
    }
}