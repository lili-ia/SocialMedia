using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.GetAllForPost;

public class GetAllCommentsForPostCommandHandler(
    ILogger<GetAllCommentsForPostCommandHandler> logger,
    ICommentRepository commentRepository,
    IPostRepository postRepository,
    IFileStorageService storageService,
    IBlockCacheService blockCacheService)
    : IRequestHandler<GetAllCommentsForPostCommand, Result<IReadOnlyList<CommentWithAuthorDto>>>
{
    public async Task<Result<IReadOnlyList<CommentWithAuthorDto>>> Handle(GetAllCommentsForPostCommand request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdWithAuthorAsync(request.PostId, ct);
        
        if (post is null || !post.CanUserAccess(request.TargetUserId))
        {
            logger.LogWarning("Post {PostId} not found or user can not access it.", request.PostId);
            
            return Result<IReadOnlyList<CommentWithAuthorDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (request.TargetUserId != post.UserId)
        {
            var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.TargetUserId, ct);

            if (blockedIds.Contains(post.UserId))
            {
                logger.LogInformation("There is a block between {PostAuthorId} and {TargetUserId}.", 
                    post.UserId, request.TargetUserId);
                
                return Result<IReadOnlyList<CommentWithAuthorDto>>.Failure("Post not found.", ErrorType.NotFound);
            }
        }

        var skip = (request.Page - 1) * request.PageSize;
        
        var comments = await commentRepository
            .GetAllByNotBlockedUsersForPostIdAsync(
                postId: request.PostId, 
                targetUserId: request.TargetUserId, 
                selector: CommentMapper.ProjectToCommentWithAuthorDto, 
                skip: skip,
                take: request.PageSize,
                ct);
        
        foreach (var comment in comments)
        {
            var user = comment.UserPreview;
            
            if (!string.IsNullOrEmpty(user.ThumbnailProfilePicStorageKey))
            {
                user.ThumbnailProfilePicUrl = storageService.GetPresignedUrl(user.ThumbnailProfilePicStorageKey);
            }
        }
        
        logger.LogInformation("Retrieved {Count} comments to post {PostId} for user {TargetUserId}.", 
            comments.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<CommentWithAuthorDto>>.Success(comments);
    }
}