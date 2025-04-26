namespace Domain.Entities;

public class Follow
{
    public int FollowerId { get; set; }
    
    public int FolloweeId { get; set; }
    
    public DateTime FollowedAt { get; set; }
    
    public virtual User Follower { get; set; }
    
    public virtual User Followee { get; set; }
}