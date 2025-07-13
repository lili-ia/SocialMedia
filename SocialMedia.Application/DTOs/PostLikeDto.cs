namespace SocialMedia.Application.DTOs;

public class PostLikeDto
{
    public Guid UserId { get; set; }
    
    public Guid PostId { get; set; }
    
    public DateTime LikedAt { get; set; }
}