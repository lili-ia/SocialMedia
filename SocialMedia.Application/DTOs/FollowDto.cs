namespace SocialMedia.Application.DTOs;

public class FollowDto
{
    public Guid FollowerId { get; set; }
    
    public Guid FolloweeId { get; set; }
    
    public DateTime FollowedAt { get; set; }
}