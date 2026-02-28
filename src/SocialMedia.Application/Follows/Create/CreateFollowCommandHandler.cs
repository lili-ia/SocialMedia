using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Create;

public class CreateFollowCommandHandler(
    IFollowRepository followRepository,
    ILogger<CreateFollowCommandHandler> logger,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IBlockCacheService blockCacheService,
    ICacheService cache)
    : IRequestHandler<CreateFollowCommand, Result<FollowResponse>>
{
    public async Task<Result<FollowResponse>> Handle(CreateFollowCommand request, CancellationToken ct)
    {
        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.FollowerId, ct);

        if (blockedIds.Contains(request.FolloweeId))
        {
            logger.LogInformation("There is a block between {FollowerId} and {FolloweeId}.",
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Followee not found.", ErrorType.NotFound);
        }

        var followeeExists = await userRepository.ExistsAsync(request.FolloweeId, UserRole.User, ct);

        if (!followeeExists)
        {
            logger.LogWarning("User {FolloweeId} not found.", request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Followee not found.", ErrorType.NotFound);
        }
        
        var follow = Follow.Create(request.FollowerId, request.FolloweeId);

        try
        {
            await followRepository.AddAsync(follow, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("User {FollowerId} successfully followed user {FolloweeId}.",
                request.FollowerId, request.FolloweeId);

            var followerCount = await followRepository
                .GetActiveFollowerCountForUserIdAsync(request.FolloweeId, ct);

            var cacheKey = $"followers:user:{request.FolloweeId}";
            await cache.RemoveAsync(cacheKey);
            await cache.RemoveAsync($"feed:followees:user:{request.FollowerId}");
            await cache.RemoveAsync($"followers:user:{request.FolloweeId}");
            await cache.RemoveAsync($"followees:user:{request.FollowerId}");
            
            return Result<FollowResponse>.Success(new FollowResponse
            {
                FollowerId = follow.FollowerId,
                FolloweeId = follow.FolloweeId,
                FollowedAt = follow.CreatedAt,
                FolloweeFollowerCount = followerCount
            });
        }
        catch (DuplicateFollowException)
        {
            logger.LogInformation("User {FollowerId} already follows user {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("You already follow this user.", ErrorType.Conflict);
        }
    }
}