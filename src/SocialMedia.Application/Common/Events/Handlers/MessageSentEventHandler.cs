using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public sealed class MessageSentEventHandler(ICacheService cache) : INotificationHandler<MessageSentNotification>
{
    public async Task Handle(MessageSentNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        await cache.RemoveByPrefixAsync($"messages:chat:{e.ChatId}");  
    } 
}