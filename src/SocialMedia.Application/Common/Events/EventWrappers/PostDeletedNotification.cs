using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostDeletedNotification(PostDeletedEvent domainEvent) : INotification
{
    public PostDeletedEvent DomainEvent { get; } = domainEvent;
}