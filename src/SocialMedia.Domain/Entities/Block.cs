namespace Domain.Entities;

public class Block
{
    public Guid BlockerId { get; set; }
    
    public Guid BlockedId { get; set; }
    
    public DateTime BlockedAt { get; set; }
    
    public virtual User Blocker { get; set; }  = null!;

    public virtual User Blocked { get; set; } = null!;
}