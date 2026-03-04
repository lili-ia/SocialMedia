using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostUnlikedEventHandler(
    INotificationRepository notificationRepository, 
    IUnitOfWork unitOfWork,
    ICacheService cache) : INotificationHandler<PostUnlikedNotification>
{
    public async Task Handle(PostUnlikedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        await cache.RemoveByPrefixAsync($"post:{e.PostId}:likers");
        
        Expression<Func<Notification, bool>> notRead = n =>
            n.Type == NotificationType.Like 
            && n.ActorId == e.LikerId && n.EntityId == e.PostId
            && !n.IsRead;

        var unreadLikeNotifications = await notificationRepository.GetAll(notRead, ct);

        foreach (var n in unreadLikeNotifications)
        {
            n.SoftDelete();
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}