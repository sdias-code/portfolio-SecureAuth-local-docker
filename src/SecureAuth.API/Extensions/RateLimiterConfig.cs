using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SecureAuth.API.Configurations;

public static class RateLimiterConfig
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 10;
                opt.QueueLimit = 2;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }
}