using System.Linq.Expressions;
using Domain.Entities;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Common.Events.Handlers;

public class UserBlockedEventHandler(
    ICacheService cache, 
    IBlockCacheService blockCache,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) 
    : INotificationHandler<UserBlockedNotification>
{
    public async Task Handle(UserBlockedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        await Task.WhenAll(
            blockCache.InvalidateAsync(e.BlockedId, ct),
            blockCache.InvalidateAsync(e.BlockerId, ct),
            cache.RemoveByPrefixAsync($"user:{e.BlockedId}:feed:followees"),
            cache.RemoveByPrefixAsync($"user:{e.BlockerId}:feed:followees:"));
        
        Expression<Func<Notification, bool>> notRead = n =>
            n.RecipientId == e.BlockerId && n.ActorId == e.BlockedId && !n.IsRead;

        var unreadFollowNotifications = await notificationRepository.GetAll(notRead, ct);

        foreach (var n in unreadFollowNotifications)
        {
            n.SoftDelete();
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}