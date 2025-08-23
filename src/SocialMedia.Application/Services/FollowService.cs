using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;
using SocialMedia.Shared.DTOs.Follow;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Services;

public class FollowService : IFollowService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<FollowService> _logger;
    private readonly IEventProducer _eventProducer;
    private readonly IMapper _mapper;
    private readonly IUserBlockChecker _blockChecker;

    public FollowService(
        SocialMediaContext db, 
        ILogger<FollowService> logger, 
        IEventProducer eventProducer, 
        IMapper mapper, 
        IUserBlockChecker blockChecker)
    {
        _db = db;
        _logger = logger;
        _eventProducer = eventProducer;
        _mapper = mapper;
        _blockChecker = blockChecker;
    }

    public async Task<Result<FollowDto>> FollowAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        _logger.LogInformation("User with id {FollowerId} attempts to follow user with id {FolloweeId}.", followerId, followeeId);

        if (followerId == followeeId)
        {
            _logger.LogWarning("User with ID {FollowerId} attempts to follow themselves.", followerId);
            
            return Result<FollowDto>.FailureResult("You can not follow yourself.", ErrorType.Forbidden);
        }
        
        var followerUsername = await _db.Users
            .Where(u => u.Id == followerId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync(ct);

        if (followerUsername == null)
        {
            _logger.LogWarning("User with ID {FollowerId} not found.", followerId);
            
            return Result<FollowDto>.FailureResult("Follower not found.", ErrorType.NotFound);
        }

        var followeeExists = await UserExistsAsync(followeeId, ct);

        if (!followeeExists)
        {
            _logger.LogWarning("User with ID {FolloweeId} not found.", followeeId);
            
            return Result<FollowDto>.FailureResult("Followee not found.", ErrorType.NotFound);
        }

        var isBlockedByFollowee = await _blockChecker.IsBlockedAsync(followeeId, followerId, ct);
        
        if (isBlockedByFollowee)
        {
            return Result<FollowDto>.FailureResult("You can not follow this user because they blocked you.", ErrorType.Forbidden);
        }

        var hasBlockedFollowee = await _blockChecker.IsBlockedAsync(followerId, followeeId, ct);
        
        if (hasBlockedFollowee)
        {
            return Result<FollowDto>.FailureResult("You can not follow this user because you blocked them.", ErrorType.Forbidden);
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
                FollowerUsername = followerUsername,
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

    public async Task<Result<bool>> UnfollowUserAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        var follow = await _db.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId, ct);

        if (follow == null)
        {
            _logger.LogWarning("User with id {FollowerId} doesn't follow user with id {FolloweeId} or one of them doesn't exist.", followerId, followeeId);
            
            return Result<bool>.FailureResult("Follow does not exist.", ErrorType.Forbidden);
        }

        try
        {
            _db.Follows.Remove(follow);
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("User with id {FollowerId} successfully unfollowed user with id {FolloweeId}.", followeeId, followeeId);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while {FollowerId} trying to unfollow {FolloweeId}", followerId, followeeId);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> IsFollowingAsync(Guid followerId, Guid followeeId, CancellationToken ct)
    {
        var isFollowing =  await _db.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId, ct);
        
        return Result<bool>.SuccessResult(isFollowing);
    }
    
    public async Task<Result<List<UserPreviewDto>>> GetFollowersAsync(Guid userId, CancellationToken ct)
    {
        var userExists = await UserExistsAsync(userId, ct);

        if (!userExists)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<List<UserPreviewDto>>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var blockedUserIds = await _blockChecker.GetUsersBlockedOrBlockingAsync(userId, ct);
        
        var followers = await _db.Follows
            .Where(f => f.FolloweeId == userId && !blockedUserIds.Contains(f.FollowerId))
            .ProjectTo<UserPreviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        
        return Result<List<UserPreviewDto>>.SuccessResult(followers);
    }

    public async Task<Result<List<UserPreviewDto>>> GetFolloweesAsync(Guid userId, CancellationToken ct)
    {
        var userExists = await UserExistsAsync(userId, ct);

        if (!userExists)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<List<UserPreviewDto>>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var blockedUserIds = await _blockChecker.GetUsersBlockedOrBlockingAsync(userId, ct);
        
        var followees = await _db.Follows
            .Where(f => f.FollowerId == userId && !blockedUserIds.Contains(f.FolloweeId))
            .ProjectTo<UserPreviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        
        return Result<List<UserPreviewDto>>.SuccessResult(followees);
    }

    private async Task<bool> UserExistsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId, ct);
    }
}