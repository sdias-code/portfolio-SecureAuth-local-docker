using SecureAuth.Domain.Entities;

namespace SecureAuth.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task AddAsync(User user);
        
    }
}