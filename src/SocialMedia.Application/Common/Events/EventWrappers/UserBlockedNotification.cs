using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class UserBlockedNotification(UserBlockedEvent domainEvent) : INotification
{
    public UserBlockedEvent DomainEvent { get; } = domainEvent;
}