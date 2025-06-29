namespace Domain.Entities;

public class PostLike
{
    public Guid UserId { get; set; }
    
    public Guid PostId { get; set; }
    
    public DateTime LikedAt { get; set; } = DateTime.Now;
    
    public virtual User User { get; set; }
    
    public virtual Post Post { get; set; }
}