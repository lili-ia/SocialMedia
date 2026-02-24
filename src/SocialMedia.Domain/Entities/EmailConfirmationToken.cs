using Domain.Exceptions;

namespace Domain.Entities;

public sealed class EmailConfirmationToken : UserTokenBase
{
    public bool IsConfirmed { get; private set; }

    private EmailConfirmationToken() { }

    private EmailConfirmationToken(
        Guid userId,
        string token,
        DateTime expiresAt)
        : base(userId, token, expiresAt)
    {
        IsConfirmed = false;
    }

    public static EmailConfirmationToken Create(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
        {
            throw new DomainValidationException("Token expiration must be in the future.");
        }

        return new EmailConfirmationToken(userId, token, expiresAt);
    }
}