using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly SocialMediaDbContext _db;

    public PostLikeRepository(SocialMediaDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(PostLike postLike, CancellationToken cancellationToken = default)
    {
        await _db.PostLikes.AddAsync(postLike, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default)
    {
        return await _db.PostLikes
            .AnyAsync(pl => pl.UserId == likerId && pl.PostId == postId, cancellationToken);
    }

    public async Task RemoveAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default)
    {
        await _db.PostLikes
            .Where(pl => pl.UserId == likerId && pl.PostId == postId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetPostLikersAsync<TResult>(
        Guid postId, 
        Expression<Func<PostLike, bool>> filter, 
        Expression<Func<PostLike, TResult>> selector, 
        int skip = 0, 
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _db.PostLikes
            .AsNoTracking()
            .Where(pl => pl.PostId == postId);
        
        query = query.Where(filter);

        var likers = await query
            .OrderBy(pl => pl.LikedAt)
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return likers.AsReadOnly();
    }

    public async Task<bool> IsLikedByUserAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.PostLikes
            .AnyAsync(pl => pl.PostId == postId && pl.UserId == userId, cancellationToken);
    }

    public async Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await _db.PostLikes
            .AsNoTracking()
            .CountAsync(pl => pl.PostId == postId, cancellationToken);
    }
}