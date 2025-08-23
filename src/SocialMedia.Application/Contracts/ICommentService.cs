using SocialMedia.Shared.DTOs.Comment;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<CommentDto>> CreateCommentAsync(string text, Guid postId, Guid commenterId, CancellationToken ct);
    
    Task<Result<CommentDto>> GetCommentAsync(Guid commentId, Guid forUserId, CancellationToken ct);
    
    Task<Result<CommentDto>> UpdateCommentAsync(Guid commentId, string text, Guid commenterId, CancellationToken ct);
    
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid commenterId, CancellationToken ct);
    
    Task<Result<List<CommentDto>>> GetCommentsForPostAsync(Guid postId, Guid forUserId, CancellationToken ct);
}