using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Likes.DeleteLike;

public class DeletePostLikeCommandHandler(
    ILogger<DeletePostLikeCommandHandler> logger,
    IPostLikeRepository postLikeRepository,
    IPostRepository postRepository,
    IBlockRepository blockRepository)
    : IRequestHandler<DeletePostLikeCommand, Result>
{
    public async Task<Result> Handle(DeletePostLikeCommand request, CancellationToken cancellationToken)
    {
        var postAuthorId = await postRepository.GetUserIdByPostIdAsync(request.PostId, cancellationToken);

        if (postAuthorId is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await blockRepository
            .IsBlockedByEitherAsync(request.LikerId, postAuthorId.Value, cancellationToken);

        if (blockExists)
        {
            logger.LogWarning("There is a block between {LikerId} and {PostAuthorId}.", 
                request.LikerId, postAuthorId.Value);
                
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var rows = await postLikeRepository.RemoveAsync(request.LikerId, request.PostId, cancellationToken);

        if (rows == 0)
        {
            logger.LogInformation("User {LikerId} haven't liked post {PostId}.", request.LikerId, request.PostId);
            
            return Result.Failure("Like not found.", ErrorType.NotFound);
        }

        logger.LogInformation("User {LikerId} successfully unliked post {PostId} by user {PostAuthorId}.",
            request.LikerId, request.PostId, postAuthorId);

        return Result.Success();
    }
}