using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetById;

public class GetPostByIdCommandHandler(
    ILogger<GetPostByIdCommandHandler> logger,
    IBlockCacheService blockCache,
    IPostRepository postRepository,
    IPostLikeRepository postLikeRepository,
    ICacheService cache)
    : IRequestHandler<GetPostByIdCommand, Result<PostDto>>
{
    private readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<PostDto>> Handle(GetPostByIdCommand request, CancellationToken ct)
    {
        var cacheKey = $"posts:{request.PostId}";
        var cachedPost = await cache.GetAsync<PostDto>(cacheKey);

        if (cachedPost is not null)
        {
            return Result<PostDto>.Success(cachedPost);
        }
        
        var post = await postRepository.GetDetailsAsync(request.PostId, PostMapper.ProjectToDto, ct);
        
        if (post is null || (post.IsHidden && post.UserId != request.TargetUserId))
        {
            logger.LogWarning("Post {PostId} not found or is hidden and doesn't belong to user {UserId}.",
                request.PostId, request.TargetUserId);
            
            return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        if (request.TargetUserId.HasValue)
        {
            post.IsLikedByTargetUser = await postLikeRepository.IsLikedByUserAsync(request.PostId, request.TargetUserId.Value, ct);
        }
        else
        {
            post.IsLikedByTargetUser = false;
        }
        
        if (request.TargetUserId is null)
        {
            logger.LogInformation("Successfully retrieved post {PostId} details for user {UserId}.", 
                request.PostId, request.TargetUserId?.ToString() ?? "Anonymous");
            
            return Result<PostDto>.Success(post);
        }
        
        var blockedIds = await blockCache.GetBlockedAndBlockerIdsAsync(request.TargetUserId.Value, ct);

        if (!blockedIds.Contains(post.UserId))
        {
            logger.LogInformation("Successfully retrieved post {PostId} details for user {UserId}.", 
                request.PostId, request.TargetUserId);
            
            return Result<PostDto>.Success(post);
        }
        
        logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", post.UserId, request.TargetUserId.Value);

        await cache.SetAsync(cacheKey, post, Ttl, ct);
        
        return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
    }
}