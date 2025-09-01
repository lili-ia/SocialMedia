namespace Domain.Entities;

public class Follow
{
    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    
    public Guid FollowerId { get; set; }
    
    public User Follower { get; set; } = null!;

    public Guid FolloweeId { get; set; }
    
    public User Followee { get; set; } = null!;
}