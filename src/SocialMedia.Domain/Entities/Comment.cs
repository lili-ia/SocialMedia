namespace Domain.Entities;

public class Comment : BaseEntity
{
    public string Text { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public Post Post { get; set; } = null!;
    
    public User User { get; set; }
}