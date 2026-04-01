using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecureAuth.Domain.Entities;

namespace SecureAuth.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("RefreshTokens");

        // 🔥 PK
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        // 🔥 Token hash obrigatório e limitado
        entity.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(200);

        // 🔥 índice único com nome profissional
        entity.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("UX_RefreshTokens_Token");

        // 🔥 Expiração obrigatória
        entity.Property(x => x.ExpiresAt)
            .IsRequired();

        // 🔥 índice para performance (ativos do usuário)
        entity.HasIndex(x => new { x.UserId, x.RevokedAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_RevokedAt");

        // 🔥 índice para consultas por expiração
        entity.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

        // 🔥 Relacionamento com usuário
        entity.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // 🔥 Auditoria
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

        // 🔥 ReplacedByTokenHash e RevocationReason
        entity.Property(x => x.ReplacedByTokenHash)
            .HasMaxLength(200)
            .IsRequired(false);

        entity.Property(x => x.RevocationReason)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}