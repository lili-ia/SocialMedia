using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class PostLikeRepository(SocialMediaDbContext db) : IPostLikeRepository
{
    public async Task AddAsync(PostLike postLike, CancellationToken ct = default)
    {
        await db.PostLikes.AddAsync(postLike, ct);
    }
    
    public async Task<IReadOnlyList<TResult>> GetNotBlockedPostLikersAsync<TResult>(
        Guid postId, 
        Guid targetUserId,
        Expression<Func<PostLike, TResult>> selector, 
        int skip = 0, 
        int take = 20,
        CancellationToken ct = default)
    {
        var query = db.PostLikes
            .AsNoTracking()
            .Where(pl => pl.PostId == postId &&
                !db.Blocks.Any(b =>
                    (b.BlockerId == targetUserId && b.BlockedId == pl.UserId) ||
                    (b.BlockerId == pl.UserId && b.BlockedId == targetUserId)
                ));

        var likers = await query
            .OrderBy(pl => pl.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync(ct);

        return likers.AsReadOnly();
    }

    public async Task<bool> IsLikedByUserAsync(Guid postId, Guid userId, CancellationToken ct = default)
    {
        return await db.PostLikes
            .AnyAsync(pl => pl.PostId == postId && pl.UserId == userId, ct);
    }

    public async Task<int> GetLikeCountAsync(Guid postId, CancellationToken ct = default)
    {
        return await db.PostLikes
            .AsNoTracking()
            .CountAsync(pl => pl.PostId == postId, ct);
    }

    public async Task<PostLike?> GetByPostAndLikerAsync(Guid postId, Guid likerId, CancellationToken ct = default)
    {
        return await db.PostLikes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == likerId, ct);
    }
}