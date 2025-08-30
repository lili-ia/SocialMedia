namespace SocialMedia.Application.DTOs.Post;

public record class PostDto
{
    public Guid PostId { get; set; }
    
    public string? Text { get; set; } 

    public Guid UserId { get; set; }
    
    public string? Username { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CommentsCount { get; set; } = 0;

    public int LikesCount { get; set; } = 0;
    
    public bool IsLiked { get; set; }
}