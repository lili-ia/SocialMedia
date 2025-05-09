using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface ICommentService
{
    Task<Result<Comment>> CreateComment(string text, int postId, int userId, CancellationToken cancellationToken);
    
    Task<Result<Comment>> GetComment(int commentId, CancellationToken cancellationToken);
    
    Task<Result<Comment>> UpdateComment(int commentId, string text, int userId, CancellationToken cancellationToken);
    
    Task<Result<bool>> DeleteComment(int commentId, int userId, CancellationToken cancellationToken);
    
    Task<Result<List<Comment>>> GetCommentsForPost(int postId, CancellationToken cancellationToken);
    
    Task<Result<int>> GetPostCommentsCountAsync(int postId, CancellationToken cancellationToken);
    
    Task<Dictionary<int,int>> GetPostsCommentsCountsAsync(List<int> postsIds, CancellationToken cancellationToken);
}