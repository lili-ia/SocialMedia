using Domain.Entities;
using Domain.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class FollowService : IFollowService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<FollowService> _logger;
    private readonly IEventProducer _eventProducer;

    public FollowService(
        SocialMediaContext db, 
        ILogger<FollowService> logger, 
        IEventProducer eventProducer)
    {
        _db = db;
        _logger = logger;
        _eventProducer = eventProducer;
    }

    public async Task<Result<FollowDto>> FollowAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        _logger.LogInformation("User with id {FollowerId} attempts to follow user with id {FolloweeId}.", followerId, followeeId);

        if (followerId == followeeId)
        {
            _logger.LogWarning("User with ID {FollowerId} attempts to follow themselves.", followerId);
            
            return Result<FollowDto>.FailureResult("You can not follow yourself.", ErrorType.Forbidden);
        }
        
        var follower = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == followerId, ct);

        if (follower == null)
        {
            _logger.LogWarning("User with ID {FollowerId} not found.", followerId);
            
            return Result<FollowDto>.FailureResult("Follower not found.");
        }
        
        var followee = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == followeeId, ct);

        if (followee == null)
        {
            _logger.LogWarning("User with ID {FolloweeId} not found.", followeeId);
            
            return Result<FollowDto>.FailureResult("Followee not found.", ErrorType.NotFound);
        }

        var followExists = await _db.Follows
            .AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == followerId, ct);

        if (followExists)
        {
            _logger.LogWarning("User with ID {FollowerId} already follows user with id {FolloweeId}.", followerId, followeeId);
            
            return Result<FollowDto>.FailureResult("You are already following this user", ErrorType.Forbidden);
        }

        var followTime = DateTime.UtcNow;
        var newFollow = new Follow
        {
            FolloweeId = followeeId,
            FollowerId = followerId,
            FollowedAt = followTime
        };

        try
        {
            await _db.Follows.AddAsync(newFollow, ct);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {FollowerId} now follows user with id {FolloweeId}.", followerId, followeeId);
            
            var evt = new FollowedEvent
            {
                FollowerId = followerId,
                FollowerUsername = follower.Username,
                FolloweeId = followeeId,
                Timestamp = newFollow.FollowedAt,
                Type = "UserFollowed"
            };

            await _eventProducer.SendMessageAsync("follows-topic", evt, ct);
            _logger.LogInformation("Follow notification was successfully sent to followee with id {FolloweeId}.", followeeId);
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogWarning("Duplicate follow request detected: {FollowerId} → {FolloweeId}", followerId, followeeId);
            
            return Result<FollowDto>.FailureResult("You're already following this user", ErrorType.Forbidden);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while {FollowerId} trying to follow {FolloweeId}", followerId, followeeId);
            
            return Result<FollowDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        var followDto = new FollowDto
        {
            FolloweeId = followeeId,
            FollowerId = followerId,
            FollowedAt = followTime
        };
                     
        return Result<FollowDto>.SuccessResult(followDto);
    }

    public Task<Result<bool>> UnfollowUserAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsFollowingAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetFollowersAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDto>> GetFollowingAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetFollowersCountAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetFollowingCountAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}