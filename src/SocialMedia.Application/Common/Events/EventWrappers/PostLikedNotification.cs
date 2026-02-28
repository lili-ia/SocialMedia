using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public sealed class PostLikedNotification(PostLikedEvent domainEvent) : INotification
{
    public PostLikedEvent DomainEvent { get; } = domainEvent;
}