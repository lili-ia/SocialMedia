using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public class PostCommentedEventHandler(
    IUserRepository userRepository, 
    INotificationRepository notificationRepository, 
    IUnitOfWork unitOfWork,
    IPostRepository postRepository) : INotificationHandler<PostCommentedNotification>
{
    public async Task Handle(PostCommentedNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var commenterUsername = await userRepository.GetUsernameByIdAsync(e.CommenterId, ct);
        
        var notificationData = new PostCommentedNotificationData(
            e.CommenterId,
            commenterUsername ?? "",
            e.Text,
            e.PostId,
            e.Timestamp
        );
        
        var postAuthorId = await postRepository.GetUserIdByPostIdAsync(e.PostId, ct);

        if (postAuthorId is null)
        {
            return;
        }
        
        var entity = Notification.Create
            (NotificationType.PostLiked,
            JsonSerializer.Serialize(notificationData),
            postAuthorId.Value,
            e.CommenterId,
            e.CommentId);

        await notificationRepository.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}