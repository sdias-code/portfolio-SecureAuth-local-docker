using Microsoft.EntityFrameworkCore;
using SecureAuth.Domain.Entities;
using SecureAuth.Domain.Repositories;
using SecureAuth.Infrastructure.Persistence;

namespace SecureAuth.Infrastructure.Repositories
{

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            email = email.ToLower().Trim();

            return await _context.Users
                .AnyAsync(x => x.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            email = email.ToLower().Trim();

            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task AddAsync(User user)
        {
            user.Email = user.Email.ToLower().Trim();

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
        }

    }  
}