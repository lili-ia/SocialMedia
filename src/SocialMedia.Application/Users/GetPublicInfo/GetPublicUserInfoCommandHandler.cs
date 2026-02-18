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
    IBlockRepository blockRepository,
    IFileStorageService storageService)
    : IRequestHandler<GetPublicUserInfoCommand, Result<UserPublicDto>>
{
    public async Task<Result<UserPublicDto>> Handle(GetPublicUserInfoCommand request, CancellationToken cancellationToken)
    {
        if (request.ForUserId is not null)
        {
            var blockExists = await blockRepository
                .IsBlockedByEitherAsync(request.UserId, request.ForUserId.Value, cancellationToken);

            if (blockExists)
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

        logger.LogInformation("Successfully retrieved user {UserId} public profile details for user {ForUserId}.", 
            request.UserId, request.ForUserId?.ToString() ?? "Anonymous");
        
        return Result<UserPublicDto>.Success(user);
    }
}