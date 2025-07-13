namespace SocialMedia.Application.DTOs;

public class PostFeedDto
{
    public Guid PostId { get; set; }

    public string? Text { get; set; }

    public Guid UserId { get; set; }
    
    public string? Username { get; set; }

    public DateTime? CreatedAt { get; set; }
    
    public int LikesCount { get; set; }
    
    public int CommentsCount { get; set; }
}