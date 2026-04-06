using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecureAuth.Application.Common.Settings;
using SecureAuth.Infrastructure.Persistence;
using Serilog;

public static class ProgramExtensions
{
    // 🔹 Swagger setup
    public static void AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SecureAuth API",
                Version = "v1",
                Description = "API de autenticação com JWT + Refresh Token | Docker-ready"
            });
        });
    }

    public static void UseSwaggerSetup(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureAuth API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }

    // 🔹 JWT Options
    public static void AddJwtOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<JwtSettings>()
            .Bind(config.GetSection("Jwt"));
    }

    // 🔹 Middlewares globais
    public static void UseGlobalMiddlewares(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // ❌ NÃO usar HTTPS em testes
        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }
        
        app.UseRateLimiter();
    }

    // 🔹 Migrações seguras
    public static void ApplyMigrationsSafe(this WebApplication app)
    {
        var retries = 5;
        var delay = TimeSpan.FromSeconds(5);

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        for (int i = 0; i < retries; i++)
        {
            try
            {
                db.Database.Migrate();
                break;
            }
            catch
            {
                if (i == retries - 1) throw;
                Thread.Sleep(delay);
            }
        }
    }
}