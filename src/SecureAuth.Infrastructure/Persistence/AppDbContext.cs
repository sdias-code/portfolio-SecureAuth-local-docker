using Microsoft.EntityFrameworkCore;
using SecureAuth.Application.Common.Interfaces;
using SecureAuth.Domain.Entities;

namespace SecureAuth.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;
    public AppDbContext(
        DbContextOptions<AppDbContext> options, 
        ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    // =========================
    // 🔥 SAVE CHANGES OVERRIDES
    // =========================

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }

    // =========================
    // 🔥 AUDITORIA
    // =========================

    private void ApplyAuditInformation()
    {
        var utcNow = DateTime.UtcNow;

        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var userId = _currentUser?.UserId;

            // Para novos registros, seta CreatedAt e CreatedBy
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;

                if (userId.HasValue)
                    entry.Entity.CreatedBy = userId.Value;
            }

            // Para registros modificados, atualiza UpdatedAt e UpdatedBy, mas preserva CreatedAt e CreatedBy
            if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;

                entry.Entity.UpdatedAt = utcNow;

                if (userId.HasValue)
                    entry.Entity.UpdatedBy = userId.Value;
            }
        }
    }
}