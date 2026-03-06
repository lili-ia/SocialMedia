namespace SocialMedia.Application.Notifications.Models;

public record AddedToGroupChatNotificationData(
    Guid ChatId,
    string ChatName,
    Guid AddedByUserId,
    string AddedByUsername);