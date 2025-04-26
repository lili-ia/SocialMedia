namespace Domain.Entities;

public class PostLike
{
    public int UserId { get; set; }
    
    public int PostId { get; set; }
    
    public DateTime LikedAt { get; set; }
    
    public virtual User User { get; set; }
    
    public virtual Post Post { get; set; }
}