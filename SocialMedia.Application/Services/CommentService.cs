using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class CommentService : ICommentService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<CommentService> _logger;
    private readonly IMapper _mapper;
    
    public CommentService(SocialMediaContext db, ILogger<CommentService> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<Result<CommentDto>> CreateCommentAsync(string text, Guid postId, Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to create a comment with ID by user {UserId}.", userId);
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<CommentDto>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var post = await _db.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == postId, ct);

        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<CommentDto>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        var newComment = new Comment
        {
            Text = text,
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            _db.Comments.Add(newComment);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Comment {CommentId} successfully created by user {UserId}.", newComment.Id, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating comment by user {UserId}.", userId);
            
            return Result<CommentDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        var dto = MapToDto(newComment);
        
        return Result<CommentDto>.SuccessResult(dto);
    }

    public async Task<Result<CommentDto>> GetCommentAsync(Guid commentId, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to get comment with ID {CommentId}.", commentId);

        var comment = await _db.Comments
            .AsNoTracking()
            .Where(c => c.Id == commentId)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
        
        if (comment != null)
        {
            return Result<CommentDto>.SuccessResult(comment);
        }
            
        _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                
        return Result<CommentDto>.FailureResult("Comment not found.", ErrorType.NotFound);
    }

    public async Task<Result<CommentDto>> UpdateCommentAsync(Guid commentId, string text, Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to update comment with ID {CommentId} by user {UserId}.", commentId, userId);
        
        var comment = await _db.Comments.FindAsync([commentId], ct);
        
        if (comment == null)
        {
            _logger.LogWarning("Comment with ID {CommentId} not found for update by user {UserId}.", commentId, userId);
                        
            return Result<CommentDto>.FailureResult("Comment not found.", ErrorType.NotFound);
        }
        
        if (comment.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to update comment {CommentId} they do not own.", userId, commentId);
                        
            return Result<CommentDto>.FailureResult("Not allowed.", ErrorType.Forbidden);
        }
        
        comment.Text = text;
        comment.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Comment {CommentId} successfully updated by user {UserId}.", commentId, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating comment {CommentId} by user {UserId}.", commentId, userId);
            
            return Result<CommentDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
        
        var dto = MapToDto(comment);
        
        return Result<CommentDto>.SuccessResult(dto);
    }

    public async Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to delete comment with ID {CommentId} by user {UserId}.", commentId, userId);
        
        var comment = await _db.Comments.FindAsync([commentId], ct);

        if (comment == null)
        {
            _logger.LogWarning("Comment with ID {CommentId} not found for delete by user {UserId}.", commentId, userId);
                
            return Result<bool>.FailureResult("Comment not found.", ErrorType.NotFound);
        }

        if (comment.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to delete comment {CommentId} they do not own.", userId, commentId);
                
            return Result<bool>.FailureResult("Not allowed.", ErrorType.Forbidden);
        }
        
        try
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("Comment {CommentId} successfully deleted by user {UserId}.", commentId, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting a comment.");

            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
        
        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<List<CommentDto>>> GetCommentsForPostAsync(Guid postId, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to get comments for post with ID {PostId}.", postId);

        var postExists = await _db.Posts.AnyAsync(p => p.Id == postId, ct);

        if (postExists == false)
        {
            _logger.LogWarning("Post with ID {PostId} not found.", postId);
            
            return Result<List<CommentDto>>.FailureResult("Post not found.", ErrorType.NotFound);
        }

        var comments = await _db.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Result<List<CommentDto>>.SuccessResult(comments);
    }
    
    private CommentDto MapToDto(Comment comment) => new()
    {
        Text = comment.Text,
        UserId = comment.UserId,
        Username = comment.User!.Username,
        PostId = comment.PostId,
        CreatedAt = comment.CreatedAt
    };
}