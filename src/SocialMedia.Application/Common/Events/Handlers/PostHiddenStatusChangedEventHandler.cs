using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostHiddenStatusChangedEventHandler(ICacheService cache) : INotificationHandler<PostHiddenStatusChangedNotification>
{
    public async Task Handle(PostHiddenStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;
        
        await Task.WhenAll(
            cache.RemoveByPrefixAsync("feed:popular"),
            cache.RemoveAsync($"user:{e.AuthorId}:posts")
        );
    }
}