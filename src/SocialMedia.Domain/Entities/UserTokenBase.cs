namespace Domain.Entities;

public class UserTokenBase : BaseEntity
{
    public Guid UserId { get; set; }
    
    public string Token { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsRevoked { get; set; }
    
    public User User { get; set; } = null!;
}