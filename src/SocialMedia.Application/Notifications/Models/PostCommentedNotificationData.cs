namespace SocialMedia.Application.Notifications.Models;

public class PostCommentedNotificationData
{
    public Guid CommenterId { get; init; }
    
    public string CommenterUsername { get; init; } = null!;
    
    public string Text { get; init; } = null!;
    
    public Guid PostId { get; init; }
    
    public DateTime CommentedAt { get; init; } 
}