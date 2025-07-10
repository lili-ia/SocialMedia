using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class NotificationService : INotificationService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<NotificationService> _logger;
    private readonly INotificationSender _notificationSender;
    
    public NotificationService(SocialMediaContext db, ILogger<NotificationService> logger, INotificationSender notificationSender)
    {
        _db = db;
        _logger = logger;
        _notificationSender = notificationSender;
    }
    public async Task NotifyPostLikedAsync(PostLikedEvent eventData)
    {
        var notification = new Notification
        {
            Type = NotificationType.Like,
            RecipientId = eventData.ToUserId,
            IsRead = false,
            Timestamp = eventData.Timestamp,
            Data = new Dictionary<string, string>()
            {
                { "PostId", eventData.PostId.ToString() },
                { "FromUserId", eventData.FromUserId.ToString() },
                { "FromUsername", eventData.FromUsername },
                { "ToUserId", eventData.ToUserId.ToString() }
            }
        };

        try
        {
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            await _notificationSender.SendNotificationToClientAsync(notification);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to send a notification to user {eventData.ToUserId}");
        }
    }

    public async Task NotifyUserFollowedAsync(FollowedEvent eventData)
    {
        var notification = new Notification
        {
            Type = NotificationType.Follow,
            RecipientId = eventData.FolloweeId,
            IsRead = false,
            Timestamp = eventData.Timestamp,
            Data = new Dictionary<string, string>()
            {
                { "FollowerId", eventData.FollowerId.ToString() },
                { "FolloweeId", eventData.FolloweeId.ToString() },
                { "FollowerUsername", eventData.FollowerUsername }
            }
        };

        try
        {
            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
            await _notificationSender.SendNotificationToClientAsync(notification);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to send a notification to user {eventData.FolloweeId}");
        }
    }

    public Task NotifyMessageReceivedAsync(MessageReceivedEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task NotifyPostCommentedAsync(PostCommentedEvent eventData)
    {
        throw new NotImplementedException();
    }
}