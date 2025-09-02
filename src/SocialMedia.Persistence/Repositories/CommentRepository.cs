using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly SocialMediaDbContext _db;
    
    public CommentRepository(SocialMediaDbContext db)
    {
        _db = db;
    }
    
    public async Task AddAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await _db.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task<Comment?> GetByIdWithPostAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Comments
            .Include(c => c.Post)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _db.Comments
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetAllByPostIdAsync<TResult>(
        Guid postId, 
        Expression<Func<Comment, bool>>? predicate, 
        Expression<Func<Comment, TResult>> selector, 
        int skip = 0, 
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Comments.AsNoTracking()
            .Where(c => c.PostId == postId);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        var comments = await query
            .OrderBy(c => c.CreatedAt)
            .Select(selector)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return comments.AsReadOnly();
    }
}