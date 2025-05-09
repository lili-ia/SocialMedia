using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class FeedService : IFeedService
{
    private readonly SocialMediaContext _db;
    private readonly ILikeService _likeService;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    private const double FollowRatio = 0.7;

    public FeedService(
        SocialMediaContext db,
        ILikeService likeService,
        ICommentService commentService,
        IMapper mapper)
    {
        _db = db;
        _likeService = likeService;
        _commentService = commentService;
        _mapper = mapper;
    }

    public async Task<List<PostFeedDto>> GetFeedAsync(int userId, CancellationToken ct, int page = 1, int pageSize = 20)
    {
        var followsIds = await _db.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FolloweeId)
            .ToListAsync(cancellationToken: ct);

        var fromFollowsCount = (int)(pageSize * FollowRatio);
        var fromPopularCount = pageSize - fromFollowsCount;

        var fromFollows = await GetRecentPostsFromUsers(followsIds, ct, page, fromFollowsCount);
        var fromPopular = await GetMostPopularPostsAsync(followsIds, DateTime.Today.AddDays(-7), page, fromPopularCount, ct);

        var combined = fromFollows.Concat(fromPopular)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return combined;
    }

    public async Task<List<PostFeedDto>> GetRecentPostsFromUsers(
        List<int> followsIds, 
        CancellationToken ct, 
        int page = 1, 
        int pageSize = 20)
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .Where(p => followsIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        if (!posts.Any())
            return new List<PostFeedDto>();

        var postIds = posts.Select(p => p.PostId).ToList();

        var likeCounts = await _likeService.GetPostsLikeCountsAsync(postIds, ct);
        var commentCounts = await _commentService.GetPostsCommentsCountsAsync(postIds, ct);
        
        var result = posts.Select(post =>
        {
            var dto = _mapper.Map<PostFeedDto>(post);
            dto.LikesCount = likeCounts.GetValueOrDefault(post.PostId, 0);
            dto.CommentsCount = commentCounts.GetValueOrDefault(post.PostId, 0);
            
            return dto;
        }).ToList();

        return result;
    }
    
    public async Task<List<PostFeedDto>> GetMostPopularPostsAsync(
        List<int> excludeUserIds,
        DateTime since,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var posts = await _db.Posts
            .Include(p => p.User)
            .Where(p => !excludeUserIds.Contains(p.UserId) && p.CreatedAt >= since)
            .OrderByDescending(p => p.PostLikes.Count)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        if (!posts.Any())
            return new List<PostFeedDto>();

        var postIds = posts.Select(p => p.PostId).ToList();

        var likeCounts = await _likeService.GetPostsLikeCountsAsync(postIds, ct);
        var commentCounts = await _commentService.GetPostsCommentsCountsAsync(postIds, ct);

        var result = posts.Select(post =>
        {
            var dto = _mapper.Map<PostFeedDto>(post);
            dto.LikesCount = likeCounts.GetValueOrDefault(post.PostId, 0);
            dto.CommentsCount = commentCounts.GetValueOrDefault(post.PostId, 0);
            return dto;
        }).ToList();

        return result;
    }
}
