using Domain.Exceptions;

namespace Domain.Entities;

public sealed class PasswordResetToken : UserTokenBase
{
    public bool IsUsed { get; private set; }

    private PasswordResetToken() { }

    private PasswordResetToken(
        Guid userId,
        string token,
        DateTime expiresAt)
        : base(userId, token, expiresAt)
    {
        IsUsed = false;
    }

    public static PasswordResetToken Create(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
        {
            throw new DomainValidationException("Password reset token must have future expiration.");
        }

        return new PasswordResetToken(userId, token, expiresAt);
    }

    // public void Use()
    // {
    //     if (IsUsed)
    //         throw new PasswordResetTokenAlreadyUsedException();
    //
    //     if (IsExpired)
    //         throw new PasswordResetTokenExpiredException();
    //
    //     IsUsed = true;
    //
    //     MarkAsUpdated();
    //
    //     AddDomainEvent(
    //         new PasswordResetRequestedEvent(UserId));
    // }
}