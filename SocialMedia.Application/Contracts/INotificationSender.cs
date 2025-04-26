using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface INotificationSender
{
    Task SendNotificationToClientAsync(Notification notification);
}