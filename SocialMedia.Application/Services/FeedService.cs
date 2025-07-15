using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class FeedService : IFeedService
{
    private readonly SocialMediaContext _db;

    private const double FollowRatio = 0.7;

    public FeedService(SocialMediaContext db)
    {
        _db = db;
    }

    public async Task<List<PostFeedDto>> GetFeedAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var followsIds = await _db.Follows
            .AsNoTracking()
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FolloweeId)
            .ToListAsync(cancellationToken: ct);

        var fromFollowsCount = (int)(pageSize * FollowRatio);
        var fromFollows = await GetRecentPostsFromUsersAsync(followsIds, fromFollowsCount, userId, ct);
        
        var fromPopularCount = pageSize - fromFollows.Count;
        var fromPopular = await GetMostPopularPostsSinceDateAsync(followsIds, DateTime.Today.AddDays(-7), fromPopularCount, userId, ct);

        var combined = fromFollows.Concat(fromPopular)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return combined;
    }

    public async Task<List<PostFeedDto>> GetRecentPostsFromUsersAsync(
        List<Guid> followsIds, 
        int fetchCount, 
        Guid forUserId,
        CancellationToken ct = default)
    {
        var viewedPostsIds = await _db.PostViews
            .AsNoTracking()
            .Where(pv => pv.UserId == forUserId)
            .Select(pv => pv.PostId)
            .ToListAsync(ct);

        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p => followsIds.Contains(p.UserId) && !viewedPostsIds.Contains(p.Id))
            .OrderByDescending(p => p.CreatedAt)
            .Take(fetchCount)
            .Select(post => new PostFeedDto
            {
                PostId = post.Id,
                Text = post.Text,
                UserId = post.User.Id,
                Username = post.User.Username,
                CreatedAt = post.CreatedAt,
                LikesCount = post.PostLikes.Count,
                CommentsCount = post.Comments.Count
            })
            .ToListAsync(ct);
        
        return posts;
    }

    public async Task<List<PostFeedDto>> GetMostPopularPostsSinceDateAsync(
        List<Guid> excludeAuthors, 
        DateTime since, 
        int fetchCount, 
        Guid forUserId,
        CancellationToken ct = default)
    {
        var viewedPostsIds = await _db.PostViews
            .AsNoTracking()
            .Where(pv => pv.UserId == forUserId)
            .Select(pv => pv.PostId)
            .ToListAsync(ct);

        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p => !excludeAuthors.Contains(p.UserId) && !viewedPostsIds.Contains(p.Id))
            .OrderByDescending(p => p.PostLikes.Count)
            .ThenByDescending(p => p.CreatedAt)
            .Take(fetchCount)
            .Select(post => new PostFeedDto
            {
                PostId = post.Id,
                Text = post.Text,
                UserId = post.User.Id,
                Username = post.User.Username,
                CreatedAt = post.CreatedAt,
                LikesCount = post.PostLikes.Count,
                CommentsCount = post.Comments.Count
            })
            .ToListAsync(ct);
        
        return posts;
    }
}
