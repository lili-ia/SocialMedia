using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Services;

public class NotificationService : INotificationService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<NotificationService> _logger;
    
    public NotificationService(SocialMediaContext db, ILogger<NotificationService> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task NotifyPostLiked(PostLikedEvent eventData)
    {
        var notification = new Notification
        {
            Type = "PostLiked",
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
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to send a notification to user {eventData.ToUserId}");
        }
    }

    public Task NotifyUserFollowed(FollowedEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task NotifyMessageReceived(MessageReceivedEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task NotifyPostCommented(PostCommentedEvent eventData)
    {
        throw new NotImplementedException();
    }
}