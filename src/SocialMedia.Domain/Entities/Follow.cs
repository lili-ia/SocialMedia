namespace Domain.Entities;

public sealed class Follow : BaseEntity
{
    public Guid FollowerId { get; set; }
    
    public User Follower { get; set; } = null!;

    public Guid FolloweeId { get; set; }
    
    public User Followee { get; set; } = null!;
}