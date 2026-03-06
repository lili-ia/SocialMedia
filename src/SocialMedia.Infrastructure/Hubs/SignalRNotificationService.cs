using Microsoft.AspNetCore.SignalR;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.DTOs.Notification;

namespace Infrastructure.Hubs;

public class SignalRNotificationService(IHubContext<ChatHub> hubContext) : IRealtimeService
{
    public async Task PushMessageAsync(Guid chatId, MessageDto message, CancellationToken ct = default)
    {
        await hubContext.Clients
            .Group(ChatHub.ChatGroup(chatId))
            .SendAsync(ChatHub.ReceiveMessageEvent, message, ct);
    }

    public async Task PushNotificationAsync(Guid userId, NotificationDto notification, CancellationToken ct = default)
    {
        await hubContext.Clients
            .Group(ChatHub.UserGroup(userId))
            .SendAsync(ChatHub.ReceiveNotificationEvent, notification, ct);
    }
}