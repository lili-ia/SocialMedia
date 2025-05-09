namespace SocialMedia.Application.DTOs;

public class PostFeedDto
{
    public int PostId { get; set; }

    public string? Text { get; set; }

    public int UserId { get; set; }
    
    public string? UserName { get; set; }

    public DateTime? CreatedAt { get; set; }
    
    public int LikesCount { get; set; }
    
    public int CommentsCount { get; set; }
}