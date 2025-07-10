using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<Comment>> CreateCommentAsync(string text, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<Comment>> GetCommentAsync(Guid commentId, CancellationToken ct);
    
    Task<Result<Comment>> UpdateCommentAsync(Guid commentId, string text, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeleteCommentAsync(Guid commentId, Guid userId, CancellationToken ct);
    
    Task<Result<List<Comment>>> GetCommentsForPostAsync(Guid postId, CancellationToken ct);
    
    Task<Result<int>> GetPostCommentsCountAsync(Guid postId, CancellationToken ct);
    
    Task<Dictionary<Guid,int>> GetPostsCommentsCountsAsync(List<Guid> postsIds, CancellationToken ct);
}