using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Like;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Likes.Create;

public class CreatePostLikeCommandHandler(
    ILogger<CreatePostLikeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IPostLikeRepository postLikeRepository,
    IPostRepository postRepository,
    IBlockRepository blockRepository,
    INotificationRepository notificationRepository)
    : IRequestHandler<CreatePostLikeCommand, Result<PostLikeResponse>>
{
    public async Task<Result<PostLikeResponse>> Handle(CreatePostLikeCommand request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdWithAuthorAsync(request.PostId, ct);

        if (post is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<PostLikeResponse>.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await blockRepository
            .IsBlockedByEitherAsync(request.LikerId, post.UserId, ct);

        if (blockExists)
        {
            logger.LogWarning("There is a block between {LikerId} and {PostAuthorId}.", 
                request.LikerId, post.UserId);
                
            return Result<PostLikeResponse>.Failure("Post not found.", ErrorType.NotFound);
        }

        var like = PostLike.Create(request.PostId, post.UserId, request.LikerId, post.User.UsernameNormalized);
        
        var notificationData = new PostLikedNotificationData
        {
            LikerId = request.LikerId,
            LikerUsername =  post.User.UsernameNormalized,
            PostId = request.PostId
        };
        
        var notification = new Notification
        {
            Type = NotificationType.Like,
            IsRead = false,
            Data = JsonSerializer.Serialize(notificationData),  
            RecipientId = post.UserId,
        };

        try
        {
            await postLikeRepository.AddAsync(like, ct);
            await notificationRepository.AddAsync(notification, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("User {LikerId} successfully liked post {PostId} by user {PostAuthorId}.",
                request.LikerId, request.PostId, post.UserId);

            var likeCount = await postLikeRepository.GetLikeCountAsync(request.PostId, ct);
            
            return Result<PostLikeResponse>.Success(new PostLikeResponse
            {
                IsLiked = true,
                LikeCount = likeCount
            });
        }
        catch (DuplicatePostLikeException)
        {
            logger.LogInformation("User {LikerId} already liked post {PostId}.", request.LikerId, request.PostId);
            
            return Result<PostLikeResponse>.Failure("You already liked this post.", ErrorType.Conflict);
        }
    }
}