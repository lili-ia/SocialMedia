using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostDeletedEventHandler(
    ICacheService cache, 
    INotificationRepository notificationRepository, 
    IUnitOfWork unitOfWork) : INotificationHandler<PostDeletedNotification>
{
    public async Task Handle(PostDeletedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        await Task.WhenAll(
            cache.RemoveByPrefixAsync("feed:popular"),
            cache.RemoveByPrefixAsync($"user:{e.UserId}:posts"));
        
        Expression<Func<Notification, bool>> notRead = n =>
            n.EntityId == e.PostId && (n.Type == NotificationType.Comment || n.Type == NotificationType.Like) && !n.IsRead;

        var unreadFollowNotifications = await notificationRepository.GetAll(notRead, ct);

        foreach (var n in unreadFollowNotifications)
        {
            n.SoftDelete();
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}