namespace Domain.Entities;

public class PostView : BaseEntity
{
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public virtual User User { get; set; } = null!;
    
    public virtual Post Post { get; set; } = null!;
}