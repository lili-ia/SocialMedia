using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.GetById;

public class GetCommentByIdCommandHandler(
    ILogger<GetCommentByIdCommandHandler> logger,
    ICommentRepository commentRepository,
    IBlockCacheService blockCacheService)
    : IRequestHandler<GetCommentByIdCommand, Result<CommentWithAuthorDto>>
{
    public async Task<Result<CommentWithAuthorDto>> Handle(GetCommentByIdCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetCommentByIdCommand {@Command}.", request);

        var comment = await commentRepository.GetByIdWithPostAsync(request.CommentId, cancellationToken);

        if (comment is null)
        {
            logger.LogWarning("Comment {CommentId} not found.", request.CommentId);

            return Result<CommentWithAuthorDto>.Failure("Comment not found.", ErrorType.NotFound);
        }
        
        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.TargetUserId, cancellationToken);

        if (blockedIds.Contains(comment.UserId))
        {
            logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.",
                comment.Post.UserId, request.TargetUserId);

            return Result<CommentWithAuthorDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        var commentDto = comment.ToDto();
        
        return Result<CommentWithAuthorDto>.Success(commentDto);
    }
}