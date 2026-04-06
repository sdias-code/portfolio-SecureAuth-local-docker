using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureAuth.Application.Common.Settings;
using SecureAuth.Infrastructure.Persistence;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // 🔐 JWT Key com fallback seguro
        var key = Environment.GetEnvironmentVariable("JWT__KEY")
            ?? "super-secret-key-test-123456789123456789";

        builder.ConfigureServices(services =>
        {
            // 🔥 Remove TODAS as configurações anteriores do DbContext
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            // 🔹 Injeta banco do Testcontainers
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            // 🔥 FORÇA configurações JWT
            services.Configure<JwtSettings>(options =>
            {
                options.Key = key;
                options.Issuer = "TestIssuer";
                options.Audience = "TestAudience";
                options.ExpirationMinutes = 60;
            });

            // 🔥 Garante que o banco está pronto (evita race conditions)
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();
        });
    }
}