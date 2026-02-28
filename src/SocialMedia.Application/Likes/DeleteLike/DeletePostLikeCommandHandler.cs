using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Likes.DeleteLike;

public class DeletePostLikeCommandHandler(
    ILogger<DeletePostLikeCommandHandler> logger,
    IPostLikeRepository postLikeRepository,
    IPostRepository postRepository,
    IBlockCacheService blockCache,
    ICacheService cache)
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
        
        var blockedIds = await blockCache.GetBlockedAndBlockerIdsAsync(request.LikerId, cancellationToken);

        if (blockedIds.Contains(postAuthorId.Value))
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
        
        var cacheKey = $"post:{request.PostId}:likers";
        await cache.RemoveAsync(cacheKey);

        logger.LogInformation("User {LikerId} successfully unliked post {PostId} by user {PostAuthorId}.",
            request.LikerId, request.PostId, postAuthorId);

        return Result.Success();
    }
}