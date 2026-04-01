using Microsoft.EntityFrameworkCore;
using SecureAuth.Infrastructure.Persistence;

namespace SecureAuth.API.Configurations;

public static class DatabaseConfig
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("PostgresConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}