using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.GetPublicInfo;

public class GetPublicUserInfoCommandHandler : IRequestHandler<GetPublicUserInfoCommand, Result<UserPublicDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetPublicUserInfoCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;

    public GetPublicUserInfoCommandHandler(
        IUserRepository userRepository, 
        ILogger<GetPublicUserInfoCommandHandler> logger, 
        IBlockRepository blockRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _blockRepository = blockRepository;
    }

    public async Task<Result<UserPublicDto>> Handle(GetPublicUserInfoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPublicUserInfoCommand {@Command}.", request);

        if (request.ForUserId is not null)
        {
            var blockExists = await _blockRepository
                .IsBlockedByEitherAsync(request.UserId, request.ForUserId.Value, cancellationToken);

            if (blockExists)
            {
                _logger.LogInformation("There is a block between {UserId} and {ForUserId}.", 
                    request.UserId, request.ForUserId);
                
                return Result<UserPublicDto>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        var user = await _userRepository.GetActiveDetailsByIdAsync(request.UserId, UserMapper.ToUserPublicDto, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UserPublicDto>.Failure("User not found.", ErrorType.NotFound);
        }

        _logger.LogInformation("Successfully retrieved user {UserId} public profile details for user {ForUserId}.", 
            request.UserId, request.ForUserId?.ToString() ?? "Anonymous");
        
        return Result<UserPublicDto>.Success(user);
    }
}