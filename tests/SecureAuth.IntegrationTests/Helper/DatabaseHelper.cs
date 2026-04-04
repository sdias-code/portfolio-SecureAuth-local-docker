using Microsoft.EntityFrameworkCore;
using SecureAuth.Infrastructure.Persistence;

namespace SecureAuth.IntegrationTests.Helper
{

    public static class DatabaseHelper
    {
        public static async Task ResetDatabaseAsync(AppDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;"
            );
        }
    }
}