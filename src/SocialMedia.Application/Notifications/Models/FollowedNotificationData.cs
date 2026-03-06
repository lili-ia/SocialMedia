namespace SocialMedia.Application.Notifications.Models;

public record FollowedNotificationData(
    Guid FollowerId, 
    string FollowerUsername, 
    DateTime FollowedAt
);