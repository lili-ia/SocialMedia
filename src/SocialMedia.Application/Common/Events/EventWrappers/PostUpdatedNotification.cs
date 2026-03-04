using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostUpdatedNotification(PostUpdatedEvent domainUpdatedEvent) : INotification
{
    public PostUpdatedEvent DomainUpdatedEvent { get; } = domainUpdatedEvent;
}