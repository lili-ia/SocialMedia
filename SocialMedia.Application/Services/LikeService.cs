using AutoMapper;
using Domain.Entities;
using Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class LikeService : ILikeService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<LikeService> _logger;
    private readonly IMapper _mapper;
    private readonly IEventProducer _eventProducer;
    
    public LikeService(SocialMediaContext db, ILogger<LikeService> logger, IMapper mapper, IEventProducer eventProducer)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _eventProducer = eventProducer;
    }
    
    public async Task<Result<PostLikeDto>> LikePostAsync(int postId, int userId, CancellationToken ct)
    {
        var post = await _db.Posts.FindAsync(postId);

        if (post == null)
        {
            return Result<PostLikeDto>.FailureResult("Couldn`t find a post with such id", ErrorType.NotFound);
        }
        
        var user = await _db.Users.FindAsync(userId);
        
        if (user == null)
        {
            return Result<PostLikeDto>.FailureResult("Couldn`t find a user with such id", ErrorType.NotFound);
        }
        
        var existingLike = _db.PostLikes.Any(pl => pl.UserId == userId && pl.PostId == postId);

        if (existingLike)
        {
            return Result<PostLikeDto>.FailureResult("You have already liked this post", ErrorType.Forbidden);
        }

        var newLike = new PostLike
        {
            UserId = userId,
            PostId = postId,
            LikedAt = DateTime.UtcNow
        };

        try
        {
            await _db.PostLikes.AddAsync(newLike, ct);
            await _db.SaveChangesAsync(ct);
            var newLikeDto = _mapper.Map<PostLikeDto>(newLike);

            var evt = new PostLikedEvent
            {
                FromUserId = userId,
                ToUserId = post.UserId,
                Timestamp = newLike.LikedAt,
                PostId = postId
            };
            
            await _eventProducer.SendMessageAsync("likes-topic", evt, ct);

            return Result<PostLikeDto>.SuccessResult(newLikeDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return Result<PostLikeDto>.FailureResult($"An error occured while liking post with id ${postId}");
        }
    }

    public async Task<Result<bool>> UnlikePostAsync(int postId, int userId, CancellationToken ct)
    {
        var post = await _db.Posts.FindAsync(postId);

        if (post == null)
        {
            return Result<bool>.FailureResult("Couldn`t find a post with such id", ErrorType.NotFound);
        }
        
        var user = await _db.Users.FindAsync(userId);
        
        if (user == null)
        {
            return Result<bool>.FailureResult("Couldn`t find a user with such id", ErrorType.NotFound);
        }
        
        var existingLike = await _db.PostLikes
            .Where(pl => pl.PostId == postId && pl.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (existingLike == null)
        {
            return Result<bool>.FailureResult("Like doesn`t exist", ErrorType.Forbidden);
        }

        try
        {
            _db.PostLikes.Remove(existingLike);
            await _db.SaveChangesAsync(ct);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return Result<bool>.FailureResult($"An error occured while unliking post with id ${postId}");
        }
    }

    public async Task<Result<bool>> IsPostLikedAsync(int postId, int userId, CancellationToken ct)
    {
        var post = await _db.Posts.FindAsync(postId);

        if (post == null)
        {
            return Result<bool>.FailureResult("Couldn`t find a post with such id", ErrorType.NotFound);
        }
        
        var user = await _db.Users.FindAsync(userId);
        
        if (user == null)
        {
            return Result<bool>.FailureResult("Couldn`t find a user with such id", ErrorType.NotFound);
        }
        
        var existingLike = _db.PostLikes.Any(pl => pl.UserId == userId && pl.PostId == postId);
        
        return Result<bool>.SuccessResult(existingLike);
    }

    public async Task<Result<int>> GetPostLikeCountAsync(int postId, CancellationToken ct)
    {
        var post = await _db.Posts
            .Include(p => p.PostLikes)
            .Where(p => p.PostId == postId)
            .FirstOrDefaultAsync(ct);
        
        if (post == null)
        {
            return Result<int>.FailureResult("Couldn`t find a post with such id", ErrorType.NotFound);
        }
        
        var count = post.PostLikes.Count;
        
        return Result<int>.SuccessResult(count); 
    }
    
    public Task<Result<int>> GetTotalLikesGivenByUserAsync(int userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<UsernameDto>>> GetUsersWhoLikedPostAsync(int postId, CancellationToken ct)
    {
        var postLikes = await _db.PostLikes
            .Include(pl => pl.User)
            .Where(pl => pl.PostId == postId)
            .ToListAsync(cancellationToken: ct);

        var usernames = _mapper.Map<List<UsernameDto>>(postLikes.Select(pl => pl.User));
        
        return Result<List<UsernameDto>>.SuccessResult(usernames);
    }
}