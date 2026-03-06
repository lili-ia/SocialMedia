using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Notifications.MarkAsRead;

public record MarkNotificationsAsReadCommand(Guid UserId) : IRequest<Result>;