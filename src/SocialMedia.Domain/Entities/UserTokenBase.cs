namespace Domain.Entities;

public class UserTokenBase : BaseEntity
{
    public string Token { get; set; } = null!;
    
    public bool IsRevoked { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;
}