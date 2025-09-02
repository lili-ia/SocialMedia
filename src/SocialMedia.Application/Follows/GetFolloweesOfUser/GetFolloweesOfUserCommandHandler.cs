using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Follows.GetFolloweesOfUser;

public class GetFolloweesOfUserCommandHandler : IRequestHandler<GetFolloweesOfUserCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private readonly IFollowRepository _followRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly ILogger<GetFolloweesOfUserCommandHandler> _logger;

    public GetFolloweesOfUserCommandHandler(
        IFollowRepository followRepository, 
        IBlockRepository blockRepository, 
        ILogger<GetFolloweesOfUserCommandHandler> logger)
    {
        _followRepository = followRepository;
        _blockRepository = blockRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetFolloweesOfUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetFolloweesForUserCommand {@Command}.", request);

        IReadOnlyList<Guid>? blockedUserIds = null;
        
        if (request.UserId != request.ForUserId && request.ForUserId is not null)
        {
            blockedUserIds = await _blockRepository.
                GetBlockedByEitherIdsAsync(request.ForUserId.Value, cancellationToken);
            
            if (blockedUserIds.Contains(request.UserId))
            {
                _logger.LogInformation("There is a block between {UserId} and {RequestUserId}.",
                    request.UserId, request.ForUserId);
            
                return Result<IReadOnlyList<UserPreviewDto>>.Failure("User not found.", ErrorType.NotFound);
            }
        }
        
        var followees = await _followRepository.GetActiveFolloweesForUserAsync(
            userId: request.UserId, 
            selector: FollowMapper.ToFolloweeUserPreviewDto, 
            excludeIds: blockedUserIds?.ToList(), 
            cancellationToken);
        
        _logger.LogInformation("Successfully retrieved {Count} followees of user {UserId} for user {ForUserId}.", 
            followees.Count, request.UserId, request.ForUserId?.ToString() ?? "Anonymous");

        return Result<IReadOnlyList<UserPreviewDto>>.Success(followees);
    }
}