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
            n.Type == NotificationType.PostLiked 
            && n.ActorId == e.LikerId && n.EntityId == e.PostId
            && !n.IsRead;

        await notificationRepository.RemoveAsync(notRead, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}