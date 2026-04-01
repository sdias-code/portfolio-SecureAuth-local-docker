namespace SecureAuth.API.Configurations;

using FluentValidation;
using MediatR;
using SecureAuth.API.Services;
using SecureAuth.Application.Auth.Commands.Register;
using SecureAuth.Application.Auth.Validators;
using SecureAuth.Application.Common.Behaviors;
using SecureAuth.Application.Common.Interfaces;
using SecureAuth.Application.Security.Interfaces;
using SecureAuth.Application.Security.Services;
using SecureAuth.Domain.Repositories;
using SecureAuth.Infrastructure.Identity;
using SecureAuth.Infrastructure.Persistence;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        // 🔐 Services (Infrastructure)
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // 🏗️ Database Context
        services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<AppDbContext>());

        // 🗄️ Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // ⚡ MediatR (registra TODOS os handlers automaticamente)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));        

        // ✅ FluentValidation (registra TODOS os validators automaticamente)
        services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

        // 🔄 Pipeline de validação automática
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPerformanceBehavior<,>));

        // 🔗 Correlation ID (para logs e rastreamento)
        services.AddHttpContextAccessor();

        // 👤 Current User Service (para acessar o usuário autenticado)        
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}