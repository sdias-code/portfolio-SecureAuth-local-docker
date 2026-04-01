using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureAuth.Domain.Entities;

namespace SecureAuth.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users");

        // 🔥 PK
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever(); // GUID controlado pela aplicação

        // 🔥 Email obrigatório e único
        entity.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        entity.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("UX_Users_Email");

        // 🔥 Password obrigatório
        entity.Property(x => x.PasswordHash)
            .IsRequired();

        // 🔥 Relacionamento com RefreshTokens
        entity.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // 🔥 AUDITORIA
        // =========================
        entity.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        entity.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        entity.Property(x => x.CreatedBy)
            .IsRequired(false);

        entity.Property(x => x.UpdatedBy)
            .IsRequired(false);

        // 🔥 Opcional: índice para consultas rápidas de usuários ativos
        entity.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");
    }
}