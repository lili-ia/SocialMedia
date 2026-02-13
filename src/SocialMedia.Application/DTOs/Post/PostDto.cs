using System.Text.Json.Serialization;

namespace SocialMedia.Application.DTOs.Post;

public record PostDto
{
    public Guid PostId { get; set; }
    
    public string? Text { get; set; } 

    public Guid UserId { get; set; }
    
    public string? Username { get; set; }

    public bool IsHidden { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int CommentCount { get; set; } = 0;

    public int LikeCount { get; set; } = 0;
    
    public int ViewCount { get; set; } = 0;
    
    public bool? IsLikedByTargetUser { get; set; }
    
    public List<string>? FileUrls { get; set; }
    
    public List<string>? FileStorageKeys { get; set; }
}