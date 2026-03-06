using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Notification;

namespace SocialMedia.Application.Notifications.Get;

public sealed record GetNotificationsCommand(
    Guid UserId,
    int Page,
    int PageSize) : IRequest<Result<NotificationsResponse>>;