using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class MessageSentEventNotification(MessageSentEvent domainEvent) : INotification
{
    public MessageSentEvent DomainEvent { get; } = domainEvent;
}