using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface ILikeService
{
    Task<Result<PostLikeDto>> LikePostAsync(int postId, int userId, CancellationToken ct);
    
    Task<Result<bool>> UnlikePostAsync(int postId, int userId, CancellationToken ct);
    
    Task<Result<bool>> IsPostLikedAsync(int postId, int userId, CancellationToken ct);
    
    Task<Dictionary<int,int>> GetPostsLikeCountsAsync(List<int> postsIds, CancellationToken ct);
    
    Task<Result<int>> GetPostLikeCountAsync(int postId, CancellationToken ct);
    
    Task<Result<int>> GetTotalLikesGivenByUserAsync(int userId, CancellationToken ct);

    Task<Result<List<UsernameDto>>> GetUsersWhoLikedPostAsync(int postId, CancellationToken ct);
}