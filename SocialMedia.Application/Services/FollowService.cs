using Domain.Entities;
using Domain.Events;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class FollowService : IFollowService
{
    private readonly SocialMediaContext _db;

    public FollowService(SocialMediaContext db)
    {
        _db = db;
    }

    public async Task<Follow?> FollowAsync(int followerId, int followeeId, CancellationToken ct)
    {
        var follower = await _db.Users.FindAsync(followerId);

        if (follower == null)
        {
            Result<FollowDto>.FailureResult("Follower was not found.", ErrorType.NotFound);
        }
        
        var followee = await _db.Users.FindAsync(followeeId);

        if (followee == null)
        {
            Result<FollowDto>.FailureResult("Followee was not found.", ErrorType.NotFound);
        }

        var followExists = await _db.Follows
            .AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == followerId, ct);

        if (followExists)
        {
            Result<FollowDto>.FailureResult("You're already following this user", ErrorType.Forbidden);
        }
        
        var newFollow = new Follow
        {
            FolloweeId = followeeId,
            FollowerId = followerId,
            FollowedAt = DateTime.UtcNow
        };
        await _db.Follows.AddAsync(newFollow, ct);
        await _db.SaveChangesAsync(ct);
            
        return newFollow;
    }
}