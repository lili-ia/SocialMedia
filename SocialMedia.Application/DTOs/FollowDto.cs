namespace SocialMedia.Application.DTOs;

public class FollowDto
{
    public int FollowerId { get; set; }
    
    public int FolloweeId { get; set; }
    
    public DateTime FollowedAt { get; set; }
}