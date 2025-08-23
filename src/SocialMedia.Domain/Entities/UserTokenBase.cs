namespace Domain.Entities;

public class UserTokenBase : BaseEntity
{
    public Guid UserId { get; set; }
    
    public string Token { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsUsed { get; set; }
    
    public virtual User User { get; set; } = null!;
}