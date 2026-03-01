using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class UnfollowedNotification(UnfollowedEvent domainEvent) : INotification
{
    public UnfollowedEvent DomainEvent { get; } = domainEvent;
}