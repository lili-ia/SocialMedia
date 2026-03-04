using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostCreatedNotification(PostCreatedEvent domainEvent) : INotification
{
    public PostCreatedEvent DomainEvent { get; } = domainEvent;
}