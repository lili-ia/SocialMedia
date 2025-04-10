using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

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
    
    public async Task<Result<Post>> CreatePost(CreatePostDto postDto, int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        
        if (user == null)
        {
            return Result<Post>.FailureResult(
                "Couldn't find a user with such id.", ErrorType.NotFound);
        }

        var newPost = _mapper.Map<Post>(postDto);
        newPost.UserId = userId;
        
        try
        {
            _db.Posts.Add(newPost);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a post.");
            
            return Result<Post>.FailureResult(
                $"An error occurred while creating this post: {e.Message}", ErrorType.ServerError);
        }

        return Result<Post>.SuccessResult(newPost);
    }
    
    public async Task<Result<Post>> GetPost(int postId)
    {
        try
        {
            var post = await _db.Posts.FindAsync(postId);
            
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

    public async Task<Result<List<Post>>> GetPostsByUserAndActiveStatus(int userId, bool isActive)
    {
        try
        {
            var posts = await _db.Posts
                .Where(p => p.UserId == userId && p.IsActive == isActive)
                .ToListAsync();
            
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

    public async Task<Result<Post>> UpdatePost(UpdatePostDto postDto)
    {
        try
        {
            var post = await _db.Posts.FindAsync(postDto.PostId);

            if (post == null)
            {
                return Result<Post>.FailureResult("Post not found.", ErrorType.NotFound);
            }
            
            _mapper.Map(postDto, post); 
            await _db.SaveChangesAsync();
            
            return Result<Post>.SuccessResult(post);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating a post.");
            
            return Result<Post>.FailureResult(
                $"An error occurred while retrieving post with id {postDto.PostId}: {e.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> DeletePost(int postId)
    {
        try
        {
            var postToDelete = await _db.Posts.FindAsync(postId);
            
            if (postToDelete == null) 
                return Result<bool>.FailureResult(
                    $"There is no posts with such id", ErrorType.NotFound);
            
            _db.Posts.Remove(postToDelete);
            await _db.SaveChangesAsync();
            
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
    
    public async Task<Result<Post>> ChangePostActiveStatus(int postId, bool activeStatus)
    {
        try
        {
            var post = await _db.Posts.FindAsync(postId);

            if (post == null)
                return Result<Post>.FailureResult(
                    $"There is no posts with such id", ErrorType.NotFound);

            post.IsActive = activeStatus;
            await _db.SaveChangesAsync();
            
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

    public async Task<Result<List<Post>>> GetPostsOfUsername(string username)
    {
        try
        {
            var user = await _db.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return Result<List<Post>>.FailureResult(
                    "Couldn't find a user with such username.", ErrorType.NotFound);
            }
                
            var posts = user.Posts.ToList();
            
            return Result<List<Post>>.SuccessResult(posts);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving {username}'s posts.");

            return Result<List<Post>>.FailureResult(
                $"An error occurred while retrieving {username}'s posts.", ErrorType.ServerError
            );
        }
    }
}