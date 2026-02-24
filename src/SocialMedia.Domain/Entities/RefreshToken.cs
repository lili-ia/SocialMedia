using Domain.Exceptions;

namespace Domain.Entities;

public sealed class RefreshToken : UserTokenBase
{
    public bool IsUsed { get; private set; }

    public string? IpAddress { get; private set; }

    public string? DeviceInfo { get; private set; }

    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { } 

    private RefreshToken(
        Guid userId,
        string token,
        DateTime expiresAt,
        string? ipAddress,
        string? deviceInfo) 
        : base(userId, token, expiresAt)
    {
        IpAddress = ipAddress;
        DeviceInfo = deviceInfo;
        IsUsed = false;
    }

    public static RefreshToken Create(
        Guid userId,
        string token,
        DateTime expiresAt,
        string? ipAddress = null,
        string? deviceInfo = null)
    {
        return new RefreshToken(
            userId,
            token,
            expiresAt,
            ipAddress,
            deviceInfo);
    }

    public void MarkAsUsed(string newToken)
    {
        if (IsUsed)
        {
            throw new SecurityDomainException("Refresh token already used.");
        }

        IsUsed = true;
        ReplacedByToken = newToken;

        MarkAsUpdated();
    }
}