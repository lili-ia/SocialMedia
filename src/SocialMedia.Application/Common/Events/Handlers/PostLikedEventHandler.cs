using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostLikedEventHandler(
    INotificationRepository notificationRepository,
    ILogger<PostLikedEventHandler> logger)
    : INotificationHandler<PostLikedEventNotification>
{
    public async Task Handle(PostLikedEventNotification notification, CancellationToken ct)
    {
        try
        {
            var notificationData = new PostLikedNotificationData
            {
                LikerId = notification.DomainEvent.FromUserId,
                LikerUsername = notification.DomainEvent.FromUsername,
                PostId = notification.DomainEvent.PostId
            };

            var entity = new Notification
            {
                Type = NotificationType.Like,
                RecipientId = notification.DomainEvent.ToUserId,
                IsRead = false,
                Data = JsonSerializer.Serialize(notificationData)
            };

            await notificationRepository.AddAsync(entity, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create notification for post like {PostId}", notification.DomainEvent.PostId);

            throw;
        }
    }
}