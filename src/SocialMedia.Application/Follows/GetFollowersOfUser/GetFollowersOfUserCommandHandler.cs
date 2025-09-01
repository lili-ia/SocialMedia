using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Follows.GetFollowersOfUser;

public class GetFollowersOfUserCommandHandler : IRequestHandler<GetFollowersOfUserCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private readonly IFollowRepository _followRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly ILogger<GetFollowersOfUserCommandHandler> _logger;

    public GetFollowersOfUserCommandHandler(
        IFollowRepository followRepository, 
        IBlockRepository blockRepository, 
        ILogger<GetFollowersOfUserCommandHandler> logger)
    {
        _followRepository = followRepository;
        _blockRepository = blockRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetFollowersOfUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetFollowersOfUserCommand {@Command}.", request);

        IReadOnlyList<Guid>? blockedUserIds = null;
        
        if (request.UserId != request.ForUserId && request.ForUserId is not null)
        {
            blockedUserIds = await _blockRepository.
                GetBlockedByEitherGuidsAsync(request.ForUserId.Value, cancellationToken);
            
            if (blockedUserIds.Contains(request.UserId))
            {
                _logger.LogInformation("There is a block between {UserId} and {RequestUserId}.",
                    request.UserId, request.ForUserId);
            
                return Result<IReadOnlyList<UserPreviewDto>>.Failure("User not found.", ErrorType.NotFound);
            }
        }
        
        var followees = await _followRepository.GetActiveFollowersForUserAsync(
            request.UserId, 
            UserMapper.ToUserPreviewDto, 
            excludeIds: blockedUserIds?.ToList(), 
            cancellationToken);
        
        _logger.LogInformation("Successfully retrieved {Count} followers of user {UserId} for user {ForUserId}.", 
            followees.Count, request.UserId, request.ForUserId?.ToString() ?? "Anonymous");

        return Result<IReadOnlyList<UserPreviewDto>>.Success(followees);
    }
}