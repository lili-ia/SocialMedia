using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Common.Events.Handlers;

public class UnfollowedEventHandler(
    ICacheService cache, 
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<UnfollowedNotification>
{
    public async Task Handle(UnfollowedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        await Task.WhenAll(
            cache.RemoveByPrefixAsync($"user:{e.FollowerId}:feed:followees"),
            cache.RemoveByPrefixAsync($"user:{e.FolloweeId}:followers"),
            cache.RemoveByPrefixAsync($"user:{e.FollowerId}:followees"));

        Expression<Func<Notification, bool>> notRead = n =>
            n.RecipientId == e.FolloweeId && n.Type == NotificationType.NewFollow && n.ActorId == e.FollowerId && !n.IsRead;

        await notificationRepository.RemoveAsync(notRead, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}