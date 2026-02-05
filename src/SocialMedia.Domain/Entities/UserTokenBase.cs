namespace Domain.Entities;

public class UserTokenBase : BaseEntity
{
    public string Token { get; set; } = null!;

    public bool IsRevoked { get; set; } = false;
    
    public DateTime? RevokedAt { get; set; }
    
    public string? ReasonForRevocation { get; set; }

    public DateTime ExpiresAt { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    public bool IsActive => !IsRevoked && !IsExpired;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}