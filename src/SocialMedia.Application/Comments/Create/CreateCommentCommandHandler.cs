using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.Create;

public class CreateCommentCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateCommentCommandHandler> logger,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IFileStorageService storageService,
    IBlockCacheService blockCacheService)
    : IRequestHandler<CreateCommentCommand, Result<CommentWithAuthorDto>>
{
    public async Task<Result<CommentWithAuthorDto>> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, ct);
        
        if (post is null || !post.CanUserComment(request.UserId))
        {
            logger.LogInformation("Post {PostId} not found or not active.", request.PostId);
            
            return Result<CommentWithAuthorDto>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        if (request.UserId != post.UserId)
        {
            var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.UserId, ct);

            if (blockedIds.Contains(post.UserId))
            {
                logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.", 
                    post.UserId, request.UserId);
        
                return Result<CommentWithAuthorDto>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
        {
            logger.LogWarning("User {UserId} attempted to create a comment but has no username.", request.UserId);
            
            return Result<CommentWithAuthorDto>.Failure("Access denied.", ErrorType.Forbidden);
        }

        var parentCommentExists = false;
        
        if (request.ParentCommentId is not null)
        {
            parentCommentExists = await commentRepository.ExistsAsync(request.ParentCommentId.Value, ct);
        }
        
        var comment = Comment.Create(
            request.UserId,
            request.PostId,
            request.Text,
            parentCommentExists ? request.ParentCommentId : null);
       
        await commentRepository.AddAsync(comment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Comment {CommentId} successfully created by user {UserId}.",
            comment.Id, request.UserId);
        
        var commentDto = comment.ToDto(user);

        if (!string.IsNullOrEmpty(commentDto.UserPreview.ThumbnailProfilePicStorageKey))
        {
            commentDto.UserPreview.ThumbnailProfilePicUrl = storageService
                .GetPresignedUrl(commentDto.UserPreview.ThumbnailProfilePicStorageKey);
        }
        
        return Result<CommentWithAuthorDto>.Success(commentDto);
    }
}