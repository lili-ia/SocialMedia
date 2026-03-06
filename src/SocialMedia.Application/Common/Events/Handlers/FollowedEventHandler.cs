using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public class FollowedEventHandler(
    ICacheService cache, 
    INotificationRepository notificationRepository, 
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : INotificationHandler<FollowedNotification>
{
    public async Task Handle(FollowedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        await Task.WhenAll(
                cache.RemoveByPrefixAsync($"user:{e.FolloweeId}:followers"),
                cache.RemoveByPrefixAsync($"user:{e.FollowerId}:feed:followees:"),
                cache.RemoveByPrefixAsync($"user:{e.FollowerId}:followees"));

        var followerUsername = await userRepository.GetUsernameByIdAsync(notification.DomainEvent.FollowerId, ct);
        
        var notificationData = new FollowedNotificationData(
            e.FollowerId,
            followerUsername ?? "",
            e.Timestamp
        );
        
        var entity = Notification.Create(
            NotificationType.NewFollow, 
            JsonSerializer.Serialize(notificationData),
            e.FolloweeId,
            e.FollowerId,
            e.FollowId);

        await notificationRepository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // TODO SEND NOTIFICATION VIA SIGNALR HUB
    }
}