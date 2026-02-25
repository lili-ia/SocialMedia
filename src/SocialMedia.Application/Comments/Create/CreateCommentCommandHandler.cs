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
    IBlockRepository blockRepository,
    IFileStorageService storageService)
    : IRequestHandler<CreateCommentCommand, Result<CommentWithAuthorDto>>
{
    public async Task<Result<CommentWithAuthorDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);
        
        if (post is null || !post.CanUserComment(request.UserId))
        {
            logger.LogInformation("Post {PostId} not found or not active.", request.PostId);
            
            return Result<CommentWithAuthorDto>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        if (request.UserId != post.UserId)
        {
            var blockExists = await blockRepository
                .IsBlockedByEitherAsync(post.UserId, request.UserId, cancellationToken);

            if (blockExists)
            {
                logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.", 
                    post.UserId, request.UserId);
                
                return Result<CommentWithAuthorDto>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User {UserId} attempted to create a comment but has no username.", request.UserId);
            
            return Result<CommentWithAuthorDto>.Failure("Access denied.", ErrorType.Forbidden);
        }

        var parentCommentExists = false;
        
        if (request.ParentCommentId is not null)
        {
            parentCommentExists = await commentRepository.ExistsAsync(request.ParentCommentId.Value, cancellationToken);
        }
        
        var comment = Comment.Create(
            request.UserId,
            request.PostId,
            request.Text,
            parentCommentExists ? request.ParentCommentId : null);
       
        await commentRepository.AddAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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