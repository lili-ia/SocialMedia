namespace Domain.Entities;

public sealed class Block : BaseEntity
{
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
    
    public Guid BlockerId { get; set; }
    
    public User Blocker { get; set; }  = null!;
    
    public Guid BlockedId { get; set; }
    
    public User Blocked { get; set; } = null!;
}