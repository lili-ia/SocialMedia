using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events.EventWrappers;

public class ProfileUpdatedNotification(ProfileUpdatedEvent domainEvent) : INotification
{
    public ProfileUpdatedEvent DomainEvent { get; } = domainEvent;
}