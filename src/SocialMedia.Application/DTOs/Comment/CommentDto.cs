namespace SocialMedia.Application.DTOs.Comment;

public class CommentDto
{
    public string Text { get; set; } = null!;

    public Guid UserId { get; set; }

    public string? Username { get; set; }
    
    public Guid PostId { get; set; }

    public DateTime CreatedAt { get; set; }
}