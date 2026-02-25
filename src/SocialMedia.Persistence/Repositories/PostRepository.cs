using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class PostRepository(SocialMediaDbContext db) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default, bool tracking = false)
    {
        var query = db.Posts
            .AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Post?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Posts
            .Include(p => p.PostFiles)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task AddAsync(Post post, CancellationToken ct = default)
    {
        await db.Posts.AddAsync(post, ct);
    }

    public async Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken ct = default)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == postId)
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<int> RemoveAsync(Guid id, CancellationToken ct)
    {
        return await db.Posts
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.IsDeleted, true)
                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), ct);
    }

    public async Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Post?> GetByIdWithAuthorAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Posts
            .AsNoTracking()
            .Include(p => p.User)
                .ThenInclude(u => u.CurrentProfilePic)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<TResult>> GetListAsync<TResult>(
        Expression<Func<Post, TResult>> selector, 
        Expression<Func<Post, bool>>? predicate, 
        Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy, 
        int? skip = null, 
        int? take = null,
        CancellationToken ct = default)
    {
        var query = db.Posts.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query
            .Select(selector)
            .ToListAsync(ct);
    }
}