using Microsoft.AspNetCore.SignalR;

namespace SocialMedia;

public class NotificationHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}