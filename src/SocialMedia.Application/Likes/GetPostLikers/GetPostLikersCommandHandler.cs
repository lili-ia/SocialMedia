using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Likes.GetPostLikers;

public class GetPostLikersCommandHandler(
    ILogger<GetPostLikersCommandHandler> logger,
    IPostLikeRepository postLikeRepository,
    IPostRepository postRepository,
    IBlockCacheService blockCache,
    ICacheService cache)
    : IRequestHandler<GetPostLikersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetPostLikersCommand request, CancellationToken ct)
    {
        var postAuthorId = await postRepository.GetUserIdByPostIdAsync(request.PostId, ct);

        if (postAuthorId is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var blockedIds = await blockCache.GetBlockedAndBlockerIdsAsync(request.TargetUserId, ct);

        if (blockedIds.Contains(postAuthorId.Value))
        {
            logger.LogWarning("There is a block between {TargetUserId} and {PostAuthorId}.", 
                request.TargetUserId, postAuthorId.Value);
                
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        var cacheKey = $"post:{request.PostId}:likers";
        var cachedLikers = await cache.GetAsync<IReadOnlyList<UserPreviewDto>>(cacheKey);

        if (cachedLikers is not null)
        {
            return Result<IReadOnlyList<UserPreviewDto>>.Success(cachedLikers);
        }
        
        var skip = (request.Page - 1) * request.PageSize;
        
        var postLikers = await postLikeRepository
            .GetNotBlockedPostLikersAsync(
                postId: request.PostId, 
                targetUserId: request.TargetUserId, 
                selector: PostLikeMapper.ProjectToUserPreviewDto, 
                skip: skip, 
                take: request.PageSize,
                ct);

        await cache.SetAsync(cacheKey, postLikers, Ttl, ct);
        
        logger.LogInformation("Retrieved {Count} likes for post {PostId} for user {TargetUserId}.", 
            postLikers.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<UserPreviewDto>>.Success(postLikers);
    }
}