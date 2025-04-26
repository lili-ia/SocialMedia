using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Services;

public class CommentService : ICommentService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<CommentService> _logger;
    
    public CommentService(SocialMediaContext db, ILogger<CommentService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<Result<Comment>> CreateComment(string text, int postId, int userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);

        if (user == null)
        {
            return Result<Comment>.FailureResult(
                "Couldn't find a user with such id.", ErrorType.NotFound);
        }
        
        var post = await _db.Posts.FindAsync(new object?[] { postId }, cancellationToken: cancellationToken);

        if (post == null)
        {
            return Result<Comment>.FailureResult(
                "Couldn't find a post with such id.", ErrorType.NotFound);
        }

        var newComment = new Comment
        {
            Text = text,
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.Now,
        };

        try
        {
            _db.Comments.Add(newComment);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a post.");
            
            return Result<Comment>.FailureResult(
                $"An error occurred while creating this comment: {e.Message}", ErrorType.ServerError);
        }

        return Result<Comment>.SuccessResult(newComment);
    }

    public async Task<Result<Comment>> GetComment(int commentId, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _db.Comments.FindAsync(new object?[] { commentId }, cancellationToken: cancellationToken);
            
            return comment != null 
                ? Result<Comment>.SuccessResult(comment) 
                : Result<Comment>.FailureResult(
                    $"Couldn't find a comment with id {commentId}.", ErrorType.NotFound);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving a comment.");
            
            return Result<Comment>.FailureResult(
                $"An error occurred while retrieving comment with id {commentId}: {e.Message}", ErrorType.ServerError);
        }
    }

    public async Task<Result<Comment>> UpdateComment(int commentId, string text, int userId, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _db.Comments.FindAsync(new object?[] { commentId }, cancellationToken: cancellationToken);

            if (comment == null)
            {
                return Result<Comment>.FailureResult("Comment not found.", ErrorType.NotFound);
            }
            
            if (comment.UserId != userId)
                return Result<Comment>.FailureResult(
                    $"Not enough permissions.", ErrorType.Forbidden);

            comment.Text = text;
            comment.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync(cancellationToken);
            
            return Result<Comment>.SuccessResult(comment);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while updating a comment.");
            
            return Result<Comment>.FailureResult(
                $"An error occurred while retrieving comment with id {commentId}: {e.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> DeleteComment(int commentId, int userId, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _db.Comments.FindAsync(new object?[] { commentId }, cancellationToken: cancellationToken);
            
            if (comment == null) 
                return Result<bool>.FailureResult(
                    $"There is no comments with such id", ErrorType.NotFound);
            
            if (comment.UserId != userId)
                return Result<bool>.FailureResult(
                    $"Not enough permissions.", ErrorType.Forbidden);
            
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting a comment.");

            return Result<bool>.FailureResult(
                $"An error occurred while deleting a comment with id {commentId}: {e.Message}",
                ErrorType.ServerError);
        }
    }

    public async Task<Result<List<Comment>>> GetCommentsForPost(int postId, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _db.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == postId, cancellationToken: cancellationToken);

            if (post == null)
            {
                return Result<List<Comment>>.FailureResult(
                    "Couldn't find a post with such id.", ErrorType.NotFound);
            }

            var comments = post.Comments.ToList();

            return Result<List<Comment>>.SuccessResult(comments);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving comments for post with id {postId}.");

            return Result<List<Comment>>.FailureResult(
                $"An error occurred while retrieving comments for post with id {postId}.", ErrorType.ServerError
            );
        }
    }
}