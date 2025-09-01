namespace Domain.Entities;

public class Comment : BaseEntity
{
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public Guid PostId { get; set; }
    
    public Post Post { get; set; } = null!;
}