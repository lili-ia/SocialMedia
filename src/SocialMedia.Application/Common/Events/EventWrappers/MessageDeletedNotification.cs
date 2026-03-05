using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class MessageDeletedNotification(MessageDeletedEvent domainEvent) : INotification
{
    public MessageDeletedEvent DomainEvent { get; } = domainEvent;
}