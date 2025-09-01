namespace Domain.Entities;

public class PostLike : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public DateTime LikedAt { get; set; }
    
    public virtual User User { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}