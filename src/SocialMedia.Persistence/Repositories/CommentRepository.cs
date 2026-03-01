using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class CommentRepository(SocialMediaDbContext db) : ICommentRepository
{
    public async Task AddAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await db.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task<Comment?> GetByIdWithPostAsync(Guid id, CancellationToken ct = default, bool tracking = false)
    {
        var query = db.Comments
            .Include(c => c.Post)
            .AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<TResult>> GetAllByPostIdAsync<TResult>(
        Guid postId, 
        Expression<Func<Comment, bool>>? predicate, 
        Expression<Func<Comment, TResult>> selector, 
        int skip = 0, 
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = db.Comments.AsNoTracking()
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

    public async Task<IReadOnlyList<TResult>> GetAllByNotBlockedUsersForPostIdAsync<TResult>(
        Guid postId, 
        Guid targetUserId,
        Expression<Func<Comment, TResult>> selector, 
        int skip = 0, 
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var comments = await db.Comments.AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PostId == postId && !c.User.BlockedUsers
                .Any(b =>
                    (b.BlockerId == targetUserId && b.BlockedId == c.UserId) ||
                    (b.BlockerId == c.UserId && b.BlockedId == targetUserId)))
            .OrderBy(c => c.CreatedAt)
            .Select(selector)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return comments.AsReadOnly();
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return db.Comments.AnyAsync(c => c.Id == id, ct);
    }
}