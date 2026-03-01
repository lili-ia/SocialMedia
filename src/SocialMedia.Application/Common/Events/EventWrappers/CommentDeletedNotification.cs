using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class CommentDeletedNotification(CommentDeletedEvent domainEvent) : INotification
{
    public CommentDeletedEvent DomainEvent { get; } = domainEvent;
}