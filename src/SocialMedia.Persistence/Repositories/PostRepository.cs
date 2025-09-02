using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class PostRepository : IPostRepository
{
    private readonly SocialMediaDbContext _db;
    
    public PostRepository(SocialMediaDbContext db)
    {
        _db = db;
    }
    
    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(bool IsActive, Guid AuthorId)?> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new { p.IsActive, p.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            return null;
        }
        
        return (post.IsActive, post.UserId);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _db.Posts.AddAsync(post, cancellationToken);
    }

    public async Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Posts
            .AsNoTracking()
            .Where(p => p.Id == postId)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        await _db.Posts
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetListAsync<TResult>(
        Expression<Func<Post, TResult>> selector, 
        Expression<Func<Post, bool>>? predicate = null, 
        Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null,
        int? skip = null, 
        int? take = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _db.Posts.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);
        
        var posts =  await query
            .Select(selector)
            .ToListAsync(cancellationToken);

        return posts.AsReadOnly();
    }

    public async Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}