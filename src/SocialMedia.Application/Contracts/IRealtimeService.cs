using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.DTOs.Notification;

namespace SocialMedia.Application.Contracts;

public interface IRealtimeService
{
    Task PushMessageAsync(Guid chatId, MessageDto message, CancellationToken ct = default);
    
    Task PushNotificationAsync(Guid userId, NotificationDto notification, CancellationToken ct = default);
}