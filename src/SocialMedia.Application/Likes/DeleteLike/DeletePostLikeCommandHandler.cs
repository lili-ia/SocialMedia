using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Likes.DeleteLike;

public class DeletePostLikeCommandHandler(
    ILogger<DeletePostLikeCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository,
    IPostLikeRepository likeRepository,
    IBlockCacheService blockCache)
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

        var like = await likeRepository.GetByPostAndLikerAsync(request.PostId, request.LikerId, cancellationToken);

        if (like is null)
        {
            logger.LogInformation("User {LikerId} haven't liked post {PostId}.", request.LikerId, request.PostId);
            
            return Result.Failure("Like not found.", ErrorType.NotFound);
        }
        
        like.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {LikerId} successfully unliked post {PostId} by user {PostAuthorId}.",
            request.LikerId, request.PostId, postAuthorId);

        return Result.Success();
    }
}