using Domain.Entities;
using Domain.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;
using SocialMedia.Shared.DTOs.Like;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Services;

public class LikeService : ILikeService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<LikeService> _logger;
    private readonly IEventProducer _eventProducer;
    private readonly IConfiguration _configuration;
    
    public LikeService(
        SocialMediaContext db, 
        ILogger<LikeService> logger, 
        IEventProducer eventProducer, 
        IConfiguration configuration)
    {
        _db = db;
        _logger = logger;
        _eventProducer = eventProducer;
        _configuration = configuration;
    }
    
    public async Task<Result<PostLikeDto>> LikePostAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("User with id {FollowerId} attempts to like post with id {PostId}.", userId, postId);
        
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == postId, ct);

        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<PostLikeDto>.FailureResult("Post not found.", ErrorType.NotFound);
        }
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PostLikeDto>.FailureResult("User not found.", ErrorType.NotFound);
        }
        
        var existingLike = await _db.PostLikes
            .AnyAsync(pl => pl.UserId == userId && pl.PostId == postId, cancellationToken: ct);

        if (existingLike)
        {
            _logger.LogWarning("Post with ID {PostId} already liked by user with ID {UserId}.", postId, userId);

            return Result<PostLikeDto>.FailureResult("You have already liked this post.", ErrorType.Forbidden);
        }

        var likedAt = DateTime.UtcNow;
        
        var newLike = new PostLike
        {
            UserId = userId,
            PostId = postId,
            LikedAt = likedAt
        };

        try
        {
            await _db.PostLikes.AddAsync(newLike, ct);
            await _db.SaveChangesAsync(ct);
            
            var evt = new PostLikedEvent
            {
                FromUserId = userId,
                ToUserId = post.UserId,
                Timestamp = newLike.LikedAt,
                PostId = postId
            };
            var topicName = _configuration["Kafka:Topics:PostLiked"];
            
            await _eventProducer.SendMessageAsync(topicName!, evt, ct);
            _logger.LogInformation("User with id {UserId} successfully liked post with id {PostId}.", userId, postId);
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogWarning("Duplicate like detected for user {UserId} and post {PostId}", userId, postId);
            
            return Result<PostLikeDto>.FailureResult("You have already liked this post.", ErrorType.Forbidden);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to like post with id {PostId}.", userId, postId);
            
            return Result<PostLikeDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
        
        var newLikeDto = new PostLikeDto
        {
            UserId = userId,
            PostId = postId,
            LikedAt = likedAt
        };
        
        return Result<PostLikeDto>.SuccessResult(newLikeDto);
    }

    public async Task<Result<bool>> UnlikePostAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == postId, ct);

        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<bool>.FailureResult("Post not found.", ErrorType.NotFound);
        }
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<bool>.FailureResult("User not found.", ErrorType.NotFound);
        }
        
        var existingLike = await _db.PostLikes
            .Where(pl => pl.PostId == postId && pl.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (existingLike == null)
        {
            _logger.LogWarning("Post with ID {PostId} isn't liked yet by user with ID {UserId}.", postId, userId);
            
            return Result<bool>.FailureResult("Like doesn't exist.", ErrorType.Forbidden);
        }

        try
        {
            _db.PostLikes.Remove(existingLike);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {UserId} successfully unliked post with id {PostId}.", userId, postId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to unlike post with id {PostId}.", userId, postId);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
        
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<bool>> IsPostLikedAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId, ct);
        
        if (!postExists)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<bool>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        var existingLike = await _db.PostLikes
            .AnyAsync(pl => pl.UserId == userId && pl.PostId == postId, ct);
        
        return Result<bool>.SuccessResult(existingLike);
    }

    public async Task<Result<int>> GetPostLikeCountAsync(Guid postId, CancellationToken ct)
    {
        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId, cancellationToken: ct);
        
        if (postExists == false)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<int>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        var count = await _db.PostLikes
            .AsNoTracking()
            .CountAsync(pl => pl.PostId == postId, ct);
        
        return Result<int>.SuccessResult(count); 
    }
    
    public async Task<Result<List<UserPreviewDto>>> GetUsersWhoLikedPostAsync(Guid postId, CancellationToken ct)
    {
        var postExists = await _db.Posts
            .AnyAsync(p => p.Id == postId, ct);
        
        if (postExists == false)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<List<UserPreviewDto>>.FailureResult("Post not found.", ErrorType.NotFound);
        }
        
        var usernames = await _db.PostLikes
            .AsNoTracking()
            .Where(pl => pl.PostId == postId)
            .Select(pl => new UserPreviewDto
            {
                UserId = pl.User.Id,
                Username = pl.User.Username
            })
            .ToListAsync(cancellationToken: ct);
        
        return Result<List<UserPreviewDto>>.SuccessResult(usernames);
    }
}