using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostUpdatedEventHandler(ICacheService cache) : INotificationHandler<PostUpdatedNotification>
{
    public async Task Handle(PostUpdatedNotification updatedNotification, CancellationToken cancellationToken)
    {
        var e = updatedNotification.DomainUpdatedEvent;
        
        await Task.WhenAll(
            cache.RemoveAsync($"user:{e.UserId}:posts"),
            cache.RemoveAsync($"posts:{e.PostId}")
        );
    }
}