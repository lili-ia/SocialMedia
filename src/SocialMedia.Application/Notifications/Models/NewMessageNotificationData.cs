namespace SocialMedia.Application.Notifications.Models;

public record NewMessageNotificationData(
    Guid ChatId,
    Guid MessageId,
    Guid SenderId,
    string SenderUsername,
    string? MessagePreview); 