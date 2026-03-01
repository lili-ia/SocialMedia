using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Comments.Delete;

public class DeleteCommentCommandHandler(
    ILogger<DeleteCommentCommandHandler> logger,
    ICommentRepository commentRepository,
    IBlockCacheService blockCacheService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCommentCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var comment = await commentRepository.GetByIdWithPostAsync(request.CommentId, ct, tracking: true);
        
        if (comment is null)
        {
            logger.LogWarning("Comment {CommentId} not found.", request.CommentId);
            
            return Result<MessageResponse>.Failure("Comment not found.", ErrorType.NotFound);
        }
        
        if (!comment.Post.CanUserAccess(request.UserId))
        {
            logger.LogInformation("Post {PostId} is not active.", comment.PostId);
            
            return Result<MessageResponse>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (!comment.CanUserDelete(request.UserId))
        {
            logger.LogInformation("User {UserId} can't delete comment {CommentId} they do not own.", 
                request.UserId, request.CommentId);
            
            return Result<MessageResponse>.Failure("Access denied.", ErrorType.Forbidden);
        }
        
        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.UserId, ct);

        if (blockedIds.Contains(comment.Post.UserId))
        {
            logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.", 
                comment.Post.UserId, request.UserId);
        
            return Result<MessageResponse>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        comment.SoftDelete();
        await unitOfWork.SaveChangesAsync(ct);
        
        logger.LogInformation("Comment {CommentId} successfully deleted by user {UserId}.", request.CommentId, request.UserId);
        
        return Result<MessageResponse>.Success(new MessageResponse("You successfully deleted comment."));
    }
}