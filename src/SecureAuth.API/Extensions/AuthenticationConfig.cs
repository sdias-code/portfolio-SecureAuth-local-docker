using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureAuth.Application.Common.Settings;
using System.Text;

namespace SecureAuth.API.Configurations;

public static class AuthenticationConfig
{
    public static IServiceCollection AddJwtAuthentication(
    this IServiceCollection services,
    IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        services.AddAuthorization();

        // 🔥 CONFIGURA JWT DEPOIS com DI correto
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                var keyBytes = Encoding.UTF8.GetBytes(jwt.Key);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(jwt.Issuer),
                    ValidateAudience = !string.IsNullOrEmpty(jwt.Audience),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"❌ JWT Auth failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}