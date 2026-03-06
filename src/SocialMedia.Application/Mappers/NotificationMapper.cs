using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Notification;

namespace SocialMedia.Application.Mappers;

public static class NotificationMapper
{
    public static Expression<Func<Notification, NotificationDto>> ProjectToDto => n => new NotificationDto
    {
        Id = n.Id,
        Type = n.Type,
        Payload = n.Data,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}