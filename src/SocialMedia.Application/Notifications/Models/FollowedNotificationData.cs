namespace SocialMedia.Application.Notifications.Models;

public class FollowedNotificationData
{
    public Guid FollowerId { get; init; }
    
    public string FollowerUsername { get; init; }
    
    public DateTime FollowedAt { get; init; }
}