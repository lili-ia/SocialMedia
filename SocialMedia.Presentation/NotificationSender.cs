using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using SocialMedia.Application.Contracts;

namespace SocialMedia;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToClientAsync(Notification notification)
    {
        await _hubContext.Clients.User(notification.RecipientId.ToString())
            .SendAsync("ReceiveNotification", notification);

    }
}