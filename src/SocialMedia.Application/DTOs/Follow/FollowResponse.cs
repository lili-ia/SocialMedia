namespace SocialMedia.Application.DTOs.Follow;

public class FollowResponse
{
    public Guid FollowerId { get; set; }
    
    public Guid FolloweeId { get; set; }
    
    public DateTime FollowedAt { get; set; }
    
    public int FolloweeFollowerCount { get; set; }
}