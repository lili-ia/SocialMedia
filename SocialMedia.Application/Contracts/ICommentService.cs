using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<Comment>> CreateComment(string text, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<Comment>> GetComment(Guid commentId, CancellationToken ct);
    
    Task<Result<Comment>> UpdateComment(Guid commentId, string text, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeleteComment(Guid commentId, Guid userId, CancellationToken ct);
    
    Task<Result<List<Comment>>> GetCommentsForPost(Guid postId, CancellationToken ct);
    
    Task<Result<int>> GetPostCommentsCountAsync(Guid postId, CancellationToken ct);
    
    Task<Dictionary<Guid,int>> GetPostsCommentsCountsAsync(List<Guid> postsIds, CancellationToken ct);
}