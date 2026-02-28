using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.GetPublicInfo;

public class GetPublicUserInfoCommandHandler(
    IUserRepository userRepository,
    ILogger<GetPublicUserInfoCommandHandler> logger,
    IFileStorageService storageService,
    ICacheService cache,
    IBlockCacheService blockCache)
    : IRequestHandler<GetPublicUserInfoCommand, Result<UserPublicDto>>
{
    private readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<UserPublicDto>> Handle(GetPublicUserInfoCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{request.UserId}:profile";

        var profile = await cache.GetAsync<UserPublicDto>(cacheKey);

        if (profile is not null)
        {
            return Result<UserPublicDto>.Success(profile);
        }
        
        if (request.ForUserId is not null)
        {
            var blockedIds = await blockCache.GetBlockedAndBlockerIdsAsync(request.ForUserId.Value, cancellationToken);

            if (blockedIds.Contains(request.UserId))
            {
                logger.LogInformation("There is a block between {UserId} and {ForUserId}.", 
                    request.UserId, request.ForUserId);
                
                return Result<UserPublicDto>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        var user = await userRepository.GetActiveDetailsByIdAsync(request.UserId, UserMapper.ToUserPublicDto, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UserPublicDto>.Failure("User not found.", ErrorType.NotFound);
        }

        if (user.ProfilePicStorageKey is not null)
        {
            user.ProfilePicUrl = storageService.GetPresignedUrl(user.ProfilePicStorageKey);
        }
        
        await cache.SetAsync(cacheKey, JsonSerializer.Serialize(profile), Ttl, cancellationToken);

        logger.LogInformation("Successfully retrieved user {UserId} public profile details for user {ForUserId}.", 
            request.UserId, request.ForUserId?.ToString() ?? "Anonymous");
        
        return Result<UserPublicDto>.Success(user);
    }
}