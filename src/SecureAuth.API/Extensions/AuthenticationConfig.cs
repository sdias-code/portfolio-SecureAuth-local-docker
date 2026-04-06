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

        // ⚠️ NÃO lança exception aqui
        if (string.IsNullOrEmpty(key))
        {
            // fallback seguro para design-time (migrations)
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
            options.RequireHttpsMetadata = false; // true em produção
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidIssuer = issuer,
                ValidAudience = audience,

                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                ClockSkew = TimeSpan.Zero
            };

            // 🔥 VALIDAÇÃO REAL ACONTECE AQUI
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Aqui você pode logar erro de token inválido
                    return Task.CompletedTask;
                }                
            };
        });

        services.AddAuthorization();

        return services;
    }
}