using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostLikedEventHandler(
    INotificationRepository notificationRepository,
    ICacheService cache,
    IUnitOfWork unitOfWork)
    : INotificationHandler<PostLikedNotification>
{
    public async Task Handle(PostLikedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        await cache.RemoveByPrefixAsync($"post:{e.PostId}:likers");
        
        var notificationData = new PostLikedNotificationData(
            e.LikerId,
            e.LikerUsername,
            e.PostId
        );

        var entity = Notification.Create(
            NotificationType.PostLiked,
            JsonSerializer.Serialize(notificationData), 
            e.ToUserId,
            e.LikerId,
            e.PostId);

        await notificationRepository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        // TODO SEND NOTIFICATION VIA SIGNALR HUB
    }
}