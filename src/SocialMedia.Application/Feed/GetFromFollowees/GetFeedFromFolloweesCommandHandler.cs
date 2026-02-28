using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public class GetFeedFromFolloweesCommandHandler(
    ILogger<GetFeedFromFolloweesCommandHandler> logger,
    IFollowRepository followRepository,
    IPostRepository postRepository,
    ICacheService cache,
    IBlockCacheService blockCacheService)
    : IRequestHandler<GetFeedFromFolloweesCommand, Result<IReadOnlyList<PostDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(45);
    
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetFeedFromFolloweesCommand request, CancellationToken ct)
    {
        var cacheKey = $"feed:followees:user:{request.ForUserId}:page:{request.Page}:size:{request.PageSize}";
        var cached = await cache.GetAsync<IReadOnlyList<PostDto>>(cacheKey);
        
        if (cached is not null)
        {
            return Result<IReadOnlyList<PostDto>>.Success(cached);
        }
        
        var followeesIds = await followRepository
            .GetActiveFolloweesForUserAsync(
                userId: request.ForUserId, 
                selector: f => f.FolloweeId, 
                excludeIds: null, 
                ct);

        if (followeesIds.Count == 0)
        {
            logger.LogInformation("User {ForUserId} has no followees.", request.ForUserId);
            
            return Result<IReadOnlyList<PostDto>>.Success([]);
        }
        
        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.ForUserId, ct);
        var visibleFolloweeIds = followeesIds.Where(id => !blockedIds.Contains(id)).ToList();
        
        Expression<Func<Post, bool>> filter = p =>
            !p.IsHidden 
            && visibleFolloweeIds.Contains(p.UserId) 
            && p.User.Status == UserStatus.Active;

        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderByCreatedAt = q => q
            .OrderByDescending(p => p.CreatedAt);
        
        var skip = (request.Page - 1) * request.PageSize;

        var posts = await postRepository.GetListAsync(
            selector: PostMapper.ProjectToDto,
            predicate: filter,
            orderBy: orderByCreatedAt,
            skip: skip,
            take: request.PageSize,
            ct);
        
        await cache.SetAsync(cacheKey, posts, Ttl, ct);
        
        logger.LogInformation("Retrieved {Count} posts for user {ForUserId}.", posts.Count, request.ForUserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}
