using Microsoft.EntityFrameworkCore;
using SecureAuth.Infrastructure.Persistence;

namespace SecureAuth.IntegrationTests.Factory
{
    public static class DbContextFactory
    {
        public static async Task<AppDbContext> CreateAsync(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var context = new AppDbContext(options);            

            return context;
        }
    }
}