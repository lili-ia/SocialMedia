using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;
using SocialMedia.Shared.DTOs.Post;

namespace SocialMedia.Application.Services;

public class FeedService : IFeedService
{
    private readonly SocialMediaContext _db;
    private readonly IUserBlockChecker _blockChecker;

    private const double FollowRatio = 0.7;

    public FeedService(SocialMediaContext db, IUserBlockChecker blockChecker)
    {
        _db = db;
        _blockChecker = blockChecker;
    }

    public async Task<List<PostDto>> GetFeedAsync(
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

    public async Task<List<PostDto>> GetRecentPostsFromUsersAsync(
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

        var blockedUserIds = await _blockChecker.GetUsersBlockedOrBlockingAsync(forUserId, ct);
        
        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p =>
                followsIds.Contains(p.UserId) &&
                !viewedPostsIds.Contains(p.Id) &&
                !blockedUserIds.Contains(p.UserId) &&
                p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(fetchCount)
            .Select(post => new PostDto
            {
                PostId = post.Id,
                Text = post.Text,
                UserId = post.User.Id,
                Username = post.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikesCount = post.PostLikes.Count,
                CommentsCount = post.Comments.Count
            })
            .ToListAsync(ct);
        
        return posts;
    }

    public async Task<List<PostDto>> GetMostPopularPostsSinceDateAsync(
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

        var blockedUserIds = await _blockChecker.GetUsersBlockedOrBlockingAsync(forUserId, ct);
        
        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p =>
                !excludeAuthors.Contains(p.UserId) &&
                !viewedPostsIds.Contains(p.Id) &&
                !blockedUserIds.Contains(p.UserId) &&
                p.IsActive)
            .OrderByDescending(p => p.PostLikes.Count)
            .ThenByDescending(p => p.CreatedAt)
            .Take(fetchCount)
            .Select(post => new PostDto
            {
                PostId = post.Id,
                Text = post.Text,
                UserId = post.User.Id,
                Username = post.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikesCount = post.PostLikes.Count,
                CommentsCount = post.Comments.Count
            })
            .ToListAsync(ct);
        
        return posts;
    }
}
