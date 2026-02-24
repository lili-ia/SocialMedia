using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Persistence.Repositories;

public class PostRepository(SocialMediaDbContext db) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Posts
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Post?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Posts
            .Include(p => p.PostFiles)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<(bool IsActive, Guid AuthorId)?> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new { p.IsHidden, p.UserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
        {
            return null;
        }
        
        return (post.IsHidden, post.UserId);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
    {
        await db.Posts.AddAsync(post, cancellationToken);
    }

    public async Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == postId)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken ct)
    {
        await db.Posts
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.IsDeleted, true)
                .SetProperty(p => p.DeletedAt, DateTime.UtcNow), ct);
    }

    public async Task<List<PostDto>> GetPublicOfAuthor(
        Guid authorId,
        Guid? targetUserId,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<Post> query = db.Posts
            .AsNoTracking()
            .Where(p => p.UserId == authorId && !p.IsHidden)
            .OrderByDescending(p => p.CreatedAt);

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        var posts = await query
            .Select(p => new PostDto
            {
                PostId = p.Id,
                Text = p.Text,
                UserId = p.UserId,
                Username = p.User.UsernameNormalized,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CommentCount = p.Comments.Count(),
                LikeCount = p.PostLikes.Count(),
                ViewCount = p.PostViews.Count(),
                FileStorageKeys = p.PostFiles.Select(f => f.OriginalStorageKey).ToList(),
                IsLikedByTargetUser = targetUserId.HasValue && p.PostLikes.Any(l => l.UserId == targetUserId.Value)
            })
            .ToListAsync(ct);

        return posts;
    }

    public Task<List<TResult>> GetHiddenOfAuthor<TResult>(Guid authorId, Expression<Func<Post, TResult>> selector, int? skip = null, int? take = null,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Post?> GetByIdWithAuthorAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}