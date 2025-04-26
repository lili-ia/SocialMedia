using AutoMapper;
using Domain.Entities;
using Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class FollowService : IFollowService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<FollowService> _logger;
    private readonly IMapper _mapper;
    private readonly IEventProducer _eventProducer;

    public FollowService(
        SocialMediaContext db, 
        ILogger<FollowService> logger, 
        IMapper mapper,
        IEventProducer eventProducer)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _eventProducer = eventProducer;
    }

    public async Task<Result<FollowDto>> FollowAsync(int followerId, int followeeId, CancellationToken ct)
    {
        var follower = await _db.Users.FindAsync(followerId);

        if (follower == null)
        {
            return Result<FollowDto>.FailureResult("Follower was not found.");
        }
        
        var followee = await _db.Users.FindAsync(followeeId);

        if (followee == null)
        {
            return Result<FollowDto>.FailureResult("Followee was not found.", ErrorType.NotFound);
        }

        var followExists = await _db.Follows
            .AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == followerId, ct);

        if (followExists)
        {
            return Result<FollowDto>.FailureResult("You're already following this user", ErrorType.Forbidden);
        }
        
        var newFollow = new Follow
        {
            FolloweeId = followeeId,
            FollowerId = followerId,
            FollowedAt = DateTime.UtcNow
        };

        try
        {
            await _db.Follows.AddAsync(newFollow, ct);
            await _db.SaveChangesAsync(ct);

            var evt = new FollowedEvent
            {
                FollowerId = followerId,
                FollowerUsername = follower.Username,
                FolloweeId = followeeId,
                Timestamp = newFollow.FollowedAt,
                Type = "UserFollowed"
            };

            await _eventProducer.SendMessageAsync("follows-topic", evt, ct);
            
            var followDto = _mapper.Map<FollowDto>(newFollow);
            
            return Result<FollowDto>.SuccessResult(followDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while {FollowerId} trying to follow {FolloweeId}", followerId, followeeId);
            
            return Result<FollowDto>.FailureResult("Internal server error.", ErrorType.ServerError);
        }
    }

    public Task<Result<bool>> UnfollowUserAsync(int followerId, int followeeId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsFollowingAsync(int followerId, int followeeId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetFollowersAsync(int userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetFollowingAsync(int userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetFollowersCountAsync(int userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetFollowingCountAsync(int userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}