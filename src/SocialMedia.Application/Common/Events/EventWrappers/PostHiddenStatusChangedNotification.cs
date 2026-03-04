using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostHiddenStatusChangedNotification(PostHiddenStatusChangedEvent domainEvent) : INotification
{
    public PostHiddenStatusChangedEvent DomainEvent { get; } = domainEvent;
}