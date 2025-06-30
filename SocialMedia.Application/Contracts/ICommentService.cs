using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<Comment>> CreateComment(string text, Guid postId, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<Comment>> GetComment(Guid commentId, CancellationToken cancellationToken);
    
    Task<Result<Comment>> UpdateComment(Guid commentId, string text, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<bool>> DeleteComment(Guid commentId, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<List<Comment>>> GetCommentsForPost(Guid postId, CancellationToken cancellationToken);
    
    Task<Result<int>> GetPostCommentsCountAsync(Guid postId, CancellationToken cancellationToken);
    
    Task<Dictionary<Guid,int>> GetPostsCommentsCountsAsync(List<Guid> postsIds, CancellationToken cancellationToken);
}