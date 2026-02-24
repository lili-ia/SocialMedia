namespace SocialMedia.Application.Notifications.Models;

public sealed class PostLikedNotificationData
{
    public Guid LikerId { get; init; }
    
    public string LikerUsername { get; init; } = null!;
    
    public Guid PostId { get; init; }
}