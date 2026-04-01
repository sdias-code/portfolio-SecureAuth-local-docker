namespace SecureAuth.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid Id { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public string? RevocationReason { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public bool IsActiveToken()
    {
        return RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }

    public void Revoke(string? replacedByTokenHash = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}