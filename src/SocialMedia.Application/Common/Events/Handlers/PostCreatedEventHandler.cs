using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostCreatedEventHandler(ICacheService cache) : INotificationHandler<PostCreatedNotification>
{
    public async Task Handle(PostCreatedNotification notification, CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;
        
        var cacheKey = $"user:{e.AuthorId}:posts";
        await cache.RemoveAsync(cacheKey);
    }
}