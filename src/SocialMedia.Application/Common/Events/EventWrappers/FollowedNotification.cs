using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class FollowedNotification(FollowedEvent domainEvent) : INotification
{
    public FollowedEvent DomainEvent { get; } = domainEvent;
}