using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;
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
    //todo: allow media in posts
    public async Task<Result<Guid>> CreatePostAsync(CreatePostRequest request, Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("User with id {UserId} attempts to create a new post.", userId);
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<Guid>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var newPost = new Post
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            UserId = userId
        };
        
        try
        {
            _db.Posts.Add(newPost);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {UserId} successfully created new post with id {PostId}.", userId, newPost.Id);
            
            return Result<Guid>.SuccessResult(newPost.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while {UserId} trying to create a new post.", userId);
            
            return Result<Guid>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }
    
    public async Task<Result<PostDto>> GetPostByIdAsync(Guid postId, Guid forUserId, CancellationToken ct)
    {
        var post = await _db.Posts
            .AsNoTracking()
            .Where(post => post.Id == postId)  
            .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct); 
        
        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<PostDto>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        if (post.IsActive || forUserId == post.UserId)
        {
            return Result<PostDto>.SuccessResult(post);
        }
        
        _logger.LogWarning("Unauthorized post retrieval attempt by user {UserId} on post {PostId}.", forUserId, postId);
        
        return Result<PostDto>.FailureResult("Access forbidden.", ErrorType.Forbidden);
    }

    public async Task<Result<List<PostDto>>> GetPublicPostsByUserId(Guid userId, CancellationToken ct)
    {
        var userExists = await _db.Users
            .AnyAsync(u => u.Id == userId, ct);

        if (!userExists)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<List<PostDto>>.FailureResult("User not found.", ErrorType.NotFound);
        }
        
        var posts = await _db.Posts
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.IsActive)
            .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        
        return Result<List<PostDto>>.SuccessResult(posts);
    }

    public async Task<Result<List<PostDto>>> GetMyInactivePosts(Guid userId, CancellationToken ct)
    {
        var userExists = await _db.Users
            .AnyAsync(u => u.Id == userId, ct);

        if (!userExists)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<List<PostDto>>.FailureResult("User not found.", ErrorType.NotFound);
        }
        
        var posts = await _db.Posts
            .AsNoTracking()
            .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
            .Where(p => p.UserId == userId && !p.IsActive)
            .ToListAsync(ct);
        
        return Result<List<PostDto>>.SuccessResult(posts);
    }
    
    public async Task<Result<PostDto>> UpdatePostAsync(UpdatePostDto updatePostDto, Guid postId, Guid userId, CancellationToken ct)
    {
        var post = await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.PostLikes)
            .FirstOrDefaultAsync(p => p.Id == postId, ct);
        
        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<PostDto>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != userId)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserId} on post {PostId}.", userId, postId);
            
            return Result<PostDto>.FailureResult("You are no allowed to update this post.", ErrorType.Forbidden);
        }

        post.Text = updatePostDto.Text;
        post.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {UserId} successfully updated post with id {PostId}.", userId, post.Id);
            
            var postDto = new PostDto
            {
                PostId = post.Id,
                Text = post.Text,
                UserId = post.UserId,
                Username = post.User.Username,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsActive = post.IsActive,
                CommentsCount = post.Comments.Count,
                LikesCount = post.PostLikes.Count
            };
            
            return Result<PostDto>.SuccessResult(postDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to update a post with id {PostId}.", userId, post.Id);
            
            return Result<PostDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var post = await _db.Posts.FindAsync([postId], ct);
            
        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<bool>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != userId)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserId} on post {PostId}.", userId, postId);
            
            return Result<bool>.FailureResult("You are no allowed to update this post.", ErrorType.Forbidden);
        }
        
        try
        {
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {UserId} successfully deleted post with id {PostId}.", userId, post.Id);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to delete a post with id {PostId}.", userId, post.Id);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }
    
    public async Task<Result<bool>> ChangePostActiveStatusAsync(Guid userId, Guid postId, bool activeStatus, CancellationToken ct)
    {
        var post = await _db.Posts.FindAsync([postId], ct);
            
        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<bool>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != userId)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserId} on post {PostId}.", userId, postId);
            
            return Result<bool>.FailureResult("You are no allowed to update this post.", ErrorType.Forbidden);
        }
        
        post.IsActive = activeStatus;
        
        try
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with id {UserId} successfully changed post {PostId} active status to {PostStatus}.", userId, post.Id, post.IsActive);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to update active status of a post with id {PostId}.", userId, post.Id);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }
    
    public async Task<Result<List<PostDto>>> GetPostsOfUsernameAsync(string username, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var skip = (page - 1) * pageSize;

        var userExists = await _db.Users
            .AnyAsync(u => u.Username == username, ct);

        if (!userExists)
        {
            _logger.LogWarning("User with username {Username} not found.", username);
            
            return Result<List<PostDto>>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var posts = await _db.Posts
            .Where(p => p.User.Username == username)
            .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
            .OrderByDescending(p => p.CreatedAt)     
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return Result<List<PostDto>>.SuccessResult(posts);
    }
}