using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Common.Events.Handlers;

public class CommentDeletedEventHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork) 
    : INotificationHandler<CommentDeletedNotification>
{
    public async Task Handle(CommentDeletedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        Expression<Func<Notification, bool>> notRead = n =>
            n.EntityId == e.CommentId && n.Type == NotificationType.PostCommented && !n.IsRead;

        await notificationRepository.RemoveAsync(notRead, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}