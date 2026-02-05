namespace Domain.Entities;

public sealed class PostLike : BaseEntity
{
    public Guid UserId { get; set; }
    
    public User User { get; set; } = null!;

    public Guid PostId { get; set; }
    
    public Post Post { get; set; } = null!;
}