using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Events.Handlers;

public class ProfileUpdatedEventHandler(ICacheService cache) : INotificationHandler<ProfileUpdatedNotification>
{
    public async Task Handle(ProfileUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var e = notification.DomainEvent;
        await cache.RemoveAsync($"user:{e.UserId}:profile");
    }
}