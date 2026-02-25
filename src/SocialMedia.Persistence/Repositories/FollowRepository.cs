using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly SocialMediaDbContext _db;

    public FollowRepository(SocialMediaDbContext db)
    {
        _db = db;
    }

    public async Task RemoveMutualAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        await _db.Follows
            .Where(f =>
                f.FollowerId == followerId && f.FolloweeId == followeeId ||
                f.FollowerId == followeeId && f.FolloweeId == followerId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        return await _db.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId,
                cancellationToken);
    }

    public async Task AddAsync(Follow follow, CancellationToken cancellationToken = default)
    {
        await _db.Follows.AddAsync(follow, cancellationToken);
    }

    public async Task<int> GetActiveFollowerCountForUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Follows
            .AsNoTracking()
            .CountAsync(f => f.FolloweeId == userId, cancellationToken);
    }

    public async Task<int> RemoveAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default)
    {
        return await _db.Follows
            .Where(f => f.FollowerId == followerId && f.FolloweeId == followeeId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetActiveFolloweesForUserAsync<TResult>(
        Guid userId, 
        Expression<Func<Follow, TResult>> selector, 
        IList<Guid>? excludeIds,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Follows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId);

        if (excludeIds is not null && excludeIds.Any())
        {
            query = query.Where(f => !excludeIds.Contains(f.FolloweeId));
        }

        var followees = await query
            .Select(selector)
            .ToListAsync(cancellationToken);

        return followees.AsReadOnly();
    }

    public async Task<IReadOnlyList<TResult>> GetActiveFollowersForUserAsync<TResult>(
        Guid userId, 
        Expression<Func<Follow, TResult>> selector, 
        IList<Guid>? excludeIds,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Follows
            .AsNoTracking()
            .Where(f => f.FolloweeId == userId);
        
        if (excludeIds is not null && excludeIds.Any())
        {
            query = query.Where(f => !excludeIds.Contains(f.FollowerId));
        }
        
        var followers = await query
            .OrderByDescending(f => f.CreatedAt)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return followers.AsReadOnly();
    }
}