using System.Linq.Expressions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Notification;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Notifications.Get;

public class GetNotificationsCommandHandler(
    INotificationRepository notificationRepository,
    ILogger<GetNotificationsCommandHandler> logger)
    : IRequestHandler<GetNotificationsCommand, Result<NotificationsResponse>>
{
    public async Task<Result<NotificationsResponse>> Handle(GetNotificationsCommand request, CancellationToken ct)
    {
        var skip = (request.Page - 1) * request.PageSize;

        Expression<Func<Notification, bool>> mustBelongToUser = n => n.RecipientId == request.UserId; 
        
        var notifications = await notificationRepository.GetAll(
            mustBelongToUser, 
            NotificationMapper.ProjectToDto, 
            skip,
            request.PageSize,
            ct);

        var unreadCount = notifications.Count(n => !n.IsRead);
        
        logger.LogInformation("Retrieved {Count} notifications for user {UserId}.", notifications.Count, request.UserId);

        return Result<NotificationsResponse>.Success(new NotificationsResponse
        {
            Notifications = notifications,
            UnreadCount = unreadCount,
            TotalCount = notifications.Count
        });
    }
}