namespace SocialMedia.Application.DTOs;

public class CommentDto
{
    public string Text { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string Username { get; set; } = null!;
    
    public Guid PostId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}