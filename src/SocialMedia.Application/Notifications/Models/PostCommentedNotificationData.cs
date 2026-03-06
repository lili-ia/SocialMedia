namespace SocialMedia.Application.Notifications.Models;

public record PostCommentedNotificationData(
    Guid CommenterId, 
    string CommenterUsername, 
    string Text, 
    Guid PostId, 
    DateTime CommentedAt
);