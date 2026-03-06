namespace SocialMedia.Application.Notifications.Models;

public sealed record PostLikedNotificationData(
    Guid LikerId, 
    string LikerUsername, 
    Guid PostId
);