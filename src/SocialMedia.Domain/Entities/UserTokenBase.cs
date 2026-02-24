using Domain.Exceptions;

namespace Domain.Entities;

public abstract class UserTokenBase : BaseEntity
{
    public string Token { get; private set; } = null!;

    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsRevoked { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public string? ReasonForRevocation { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    protected UserTokenBase() { }

    protected UserTokenBase(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new DomainValidationException("Token cannot be empty.");
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new DomainValidationException("Token expiration must be in the future.");
        }

        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public void Revoke(string? reason = null)
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReasonForRevocation = reason;

        MarkAsUpdated();
    }
}