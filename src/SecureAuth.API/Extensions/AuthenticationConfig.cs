using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SecureAuth.API.Configurations;

public static class AuthenticationConfig
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var key = config["Jwt:Key"];
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];

        // 🔐 fallback seguro (design-time / testes)
        if (string.IsNullOrEmpty(key))
        {
            key = "development_super_secret_key_1234567890";
        }

        var keyBytes = Encoding.UTF8.GetBytes(key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // 🔒 true em produção
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 🔥 VALIDAÇÕES CONTROLADAS (funciona em produção e testes)
                ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
                ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidIssuer = issuer,
                ValidAudience = audience,

                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"❌ JWT Auth failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    // 🔥 validação extra de segurança (opcional mas profissional)
                    var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;

                    if (claimsIdentity == null || !claimsIdentity.Claims.Any())
                    {
                        context.Fail("Token sem claims válidas");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}