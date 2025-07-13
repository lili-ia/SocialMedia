using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class PostService : IPostService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<PostService> _logger;
    private readonly IMapper _mapper;
    
    public PostService(SocialMediaContext db, ILogger<PostService> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<Result<Post>> CreatePostAsync(CreatePostDto postDto, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);
        
        if (user == null)
        {
            return Result<Post>.FailureResult(
                "Couldn't find a user with such id.", ErrorType.NotFound);
        }

        var newPost = _mapper.Map<Post>(postDto);
        newPost.UserId = userId;
        newPost.CreatedAt = DateTime.UtcNow;
        
        try
        {
            _db.Posts.Add(newPost);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a post.");
            
            return Result<Post>.FailureResult(
                $"An error occurred while creating this post: {e.Message}", ErrorType.ServerError);
        }

        return Result<Post>.SuccessResult(newPost);
    }
    
    public async Task<Result<Post>> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _db.Posts.FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);
            
            return post != null 
                ? Result<Post>.SuccessResult(post) 
                : Result<Post>.FailureResult(
                    $"Couldn't find a post with id {postId}.", ErrorType.NotFound);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving a post.");
            
            return Result<Post>.FailureResult(
                $"An error occurred while retrieving post with id {postId}: {e.Message}", ErrorType.ServerError);
        }
    }

    public async Task<Result<List<Post>>> GetPostsByUserAndActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken)
    {
        try
        {
            var posts = await _db.Posts
                .Where(p => p.UserId == userId && p.IsActive == isActive)
                .ToListAsync(cancellationToken: cancellationToken);
            
            return Result<List<Post>>.SuccessResult(posts);
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"An error occurred while retrieving {(isActive ? "public" : "archived")} posts : {e.Message}");
            
            return Result<List<Post>>.FailureResult(
                $"An error occurred while retrieving user {userId}'s {(isActive ? "public" : "archived")} posts.",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<Post>> UpdatePostAsync(UpdatePostDto postDto, Guid postId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _db.Posts.FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);

            if (post == null)
            {
                return Result<Post>.FailureResult("Post not found.", ErrorType.NotFound);
            }
            
            if (post.UserId != userId)
                return Result<Post>.FailureResult(
                    $"Not enough permissions.", ErrorType.Forbidden);
            
            _mapper.Map(postDto, post); 
            post.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
            
            return Result<Post>.SuccessResult(post);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating a post.");
            
            return Result<Post>.FailureResult(
                $"An error occurred while retrieving post with id {postId}: {e.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _db.Posts.FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);
            
            if (post == null) 
                return Result<bool>.FailureResult(
                    $"There is no posts with such id", ErrorType.NotFound);
            
            if (post.UserId != userId)
                return Result<bool>.FailureResult(
                    $"Not enough permissions.", ErrorType.Forbidden);
            
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting a post.");

            return Result<bool>.FailureResult(
                $"An error occurred while deleting post with id {postId}: {e.Message}",
                ErrorType.ServerError);
        }
    }
    
    public async Task<Result<Post>> ChangePostActiveStatusAsync(Guid postId, bool activeStatus, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _db.Posts.FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);

            if (post == null)
                return Result<Post>.FailureResult(
                    $"There is no posts with such id", ErrorType.NotFound);

            post.IsActive = activeStatus;
            await _db.SaveChangesAsync(cancellationToken);
            
            return Result<Post>.SuccessResult(post);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting a post.");

            return Result<Post>.FailureResult(
                $"An error occurred while updating post with id {postId}: {e.Message}",
                ErrorType.ServerError);
        }
    }
    
    public async Task<Result<List<Post>>> GetPostsOfUsernameAsync(
        string username, 
        int page = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var skip = (page - 1) * pageSize;

        var userExists = await _db.Users
            .AnyAsync(u => u.Username == username, cancellationToken);

        if (!userExists)
        {
            return Result<List<Post>>.FailureResult(
                "Couldn't find a user with such username.", ErrorType.NotFound);
        }

        var posts = await _db.Posts
            .Where(p => p.User.Username == username) 
            .OrderByDescending(p => p.CreatedAt)     
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Result<List<Post>>.SuccessResult(posts);
    }
}