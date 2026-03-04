using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostUnlikedNotification(PostUnlikedEvent domainEvent) : INotification
{
    public PostUnlikedEvent DomainEvent { get; } = domainEvent;
}