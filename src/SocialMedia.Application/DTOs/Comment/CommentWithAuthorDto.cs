using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.DTOs.Comment;

public class CommentWithAuthorDto
{
    public UserPreviewDto UserPreview { get; set; } = null!;
    
    public string Text { get; set; } = null!;
    
    public Guid PostId { get; set; }

    public DateTime CreatedAt { get; set; }
}