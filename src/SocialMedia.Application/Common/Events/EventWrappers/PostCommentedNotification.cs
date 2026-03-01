using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class PostCommentedNotification(PostCommentedEvent domainEvent) : INotification
{
    public PostCommentedEvent DomainEvent { get; } = domainEvent;
}