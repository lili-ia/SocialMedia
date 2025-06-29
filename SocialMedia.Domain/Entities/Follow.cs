namespace Domain.Entities;

public class Follow
{
    public Guid FollowerId { get; set; }
    
    public Guid FolloweeId { get; set; }
    
    public DateTime FollowedAt { get; set; } = DateTime.Now;
    
    public virtual User Follower { get; set; }
    
    public virtual User Followee { get; set; }
}