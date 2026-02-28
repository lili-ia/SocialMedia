using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Follows.GetFollowersOfUser;

public class GetFollowersOfUserCommandHandler(
    IFollowRepository followRepository,
    IBlockCacheService blockCache,
    ILogger<GetFollowersOfUserCommandHandler> logger,
    ICacheService cache)
    : IRequestHandler<GetFollowersOfUserCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetFollowersOfUserCommand request, CancellationToken ct)
    {
        IReadOnlyList<Guid>? blockedUserIds = null;
        
        if (request.UserId != request.ForUserId && request.ForUserId is not null)
        {
            var blockedSet = await blockCache.GetBlockedAndBlockerIdsAsync(request.ForUserId.Value, ct);
            
            if (blockedSet.Contains(request.UserId))
            {
                logger.LogInformation("There is a block between {UserId} and {RequestUserId}.",
                    request.UserId, request.ForUserId);
            
                return Result<IReadOnlyList<UserPreviewDto>>.Failure("User not found.", ErrorType.NotFound);
            }

            blockedUserIds = blockedSet.ToList();
        }

        var cacheKey = $"followers:user:{request.ForUserId}";
        var cachedFollowers = await cache.GetAsync<IReadOnlyList<UserPreviewDto>>(cacheKey);

        if (cachedFollowers is not null)
        {
            return Result<IReadOnlyList<UserPreviewDto>>.Success(cachedFollowers);
        }
        
        var followers = await followRepository.GetActiveFollowersForUserAsync(
            request.UserId, 
            FollowMapper.ToFollowerUserPreviewDto, 
            excludeIds: blockedUserIds?.ToList(), 
            ct);

        await cache.SetAsync(cacheKey, followers, Ttl, ct);
        
        logger.LogInformation("Successfully retrieved {Count} followers of user {UserId} for user {ForUserId}.", 
            followers.Count, request.UserId, request.ForUserId?.ToString() ?? "Anonymous");

        return Result<IReadOnlyList<UserPreviewDto>>.Success(followers);
    }
}