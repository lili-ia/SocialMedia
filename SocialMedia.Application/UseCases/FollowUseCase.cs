using AutoMapper;
using Domain.Events;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.UseCases;

public class FollowUseCase : IFollowUseCase
{
    private readonly SocialMediaContext _db;
    private readonly IFollowService _followService;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IEventDispatcher _eventDispatcher;

    public FollowUseCase(
        SocialMediaContext db, 
        ILogger logger, 
        IMapper mapper, 
        IEventDispatcher eventDispatcher, 
        IFollowService followService)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _eventDispatcher = eventDispatcher;
        _followService = followService;
    }

    public async Task<Result<FollowDto>> ExecuteAsync(int followerId, int followeeId, CancellationToken ct)
    {
        try
        {
            var follow = await _followService.FollowAsync(followerId, followeeId, ct);

            if (follow == null)
            {
                return Result<FollowDto>.FailureResult("Unable to follow user.", ErrorType.NotFound);
            }

            var follower = await _db.Users.FindAsync(followerId);
            
            var evt = new FollowedEvent
            {
                FollowerId = followerId,
                FollowerUsername = follower.Username,
                FolloweeId = followeeId,
                Timestamp = follow.FollowedAt,
                Type = "UserFollowed"
            };

            await _eventDispatcher.DispatchAsync(evt);

            var followDto = _mapper.Map<FollowDto>(follow);
            
            return Result<FollowDto>.SuccessResult(followDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing follow use case.");
            
            return Result<FollowDto>.FailureResult("Internal server error.", ErrorType.ServerError);
        }
    }
}