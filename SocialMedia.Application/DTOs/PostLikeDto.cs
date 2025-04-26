namespace SocialMedia.Application.DTOs;

public class PostLikeDto
{
    public int UserId { get; set; }
    
    public int PostId { get; set; }
    
    public DateTime LikedAt { get; set; }
}