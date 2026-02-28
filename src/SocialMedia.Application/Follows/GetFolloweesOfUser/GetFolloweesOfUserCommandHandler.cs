using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Follows.GetFolloweesOfUser;

public class GetFolloweesOfUserCommandHandler(
    IFollowRepository followRepository,
    IBlockCacheService blockCacheService,
    ILogger<GetFolloweesOfUserCommandHandler> logger,
    ICacheService cache)
    : IRequestHandler<GetFolloweesOfUserCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetFolloweesOfUserCommand request, CancellationToken ct)
    {
        List<Guid>? excludeIds = null;

        if (request.ForUserId is not null)
        {
            var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.ForUserId.Value, ct);

            if (request.UserId != request.ForUserId && blockedIds.Contains(request.UserId))
            {
                logger.LogInformation("There is a block between {UserId} and {ForUserId}.",
                    request.UserId, request.ForUserId);

                return Result<IReadOnlyList<UserPreviewDto>>.Failure("User not found.", ErrorType.NotFound);
            }

            excludeIds = blockedIds.ToList();
        }

        var cacheKey = $"followees:user:{request.ForUserId}";
        var cachedFollowees = await cache.GetAsync<IReadOnlyList<UserPreviewDto>>(cacheKey);
        
        if (cachedFollowees is not null)
        {
            return Result<IReadOnlyList<UserPreviewDto>>.Success(cachedFollowees);
        }
        
        var followees = await followRepository.GetActiveFolloweesForUserAsync(
            userId: request.UserId,
            selector: FollowMapper.ToFolloweeUserPreviewDto,
            excludeIds: excludeIds,
            ct);

        await cache.SetAsync(cacheKey, followees, Ttl, ct);
        
        logger.LogInformation("Successfully retrieved {Count} followees of user {UserId} for user {ForUserId}.",
            followees.Count, request.UserId, request.ForUserId?.ToString() ?? "Anonymous");

        return Result<IReadOnlyList<UserPreviewDto>>.Success(followees);
    }
}