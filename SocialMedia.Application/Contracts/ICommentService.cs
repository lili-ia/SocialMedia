using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<CommentDto>> CreateCommentAsync(string text, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<CommentDto>> GetCommentAsync(Guid commentId, CancellationToken ct);
    
    Task<Result<CommentDto>> UpdateCommentAsync(Guid commentId, string text, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct);
    
    Task<Result<List<CommentDto>>> GetCommentsForPostAsync(Guid postId, CancellationToken ct);
}