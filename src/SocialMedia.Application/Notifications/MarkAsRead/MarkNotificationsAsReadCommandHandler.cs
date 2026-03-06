using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Notifications.MarkAsRead;

public class MarkNotificationsAsReadCommandHandler(
    INotificationRepository notificationRepository,
    ILogger<MarkNotificationsAsReadCommandHandler> logger)
    : IRequestHandler<MarkNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationsAsReadCommand request, CancellationToken ct)
    {
        await notificationRepository.MarkAllAsReadImmediatelyAsync(request.UserId, ct);

        logger.LogInformation("Marked all notifications as read for user {UserId}.", request.UserId);

        return Result.Success();
    }
}