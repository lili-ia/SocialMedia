using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class ChatParticipantAddedEventNotification(ChatParticipantAddedEvent domainEvent) : INotification
{
    public ChatParticipantAddedEvent DomainEvent { get; } = domainEvent;
}