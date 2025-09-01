namespace Domain.Entities;

public class Follow
{
    public Guid FollowerId { get; set; }

    public Guid FolloweeId { get; set; }

    public DateTime FollowedAt { get; set; }
    
    public User Follower { get; set; } = null!;

    public User Followee { get; set; } = null!;
}