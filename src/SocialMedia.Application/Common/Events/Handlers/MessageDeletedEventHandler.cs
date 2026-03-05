using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public class MessageDeletedEventHandler(ICacheService cache) : INotificationHandler<MessageDeletedNotification>
{
    public async Task Handle(MessageDeletedNotification notification, CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;
        
        await cache.RemoveByPrefixAsync($"messages:chat:{e.ChatId}");  
    }
}