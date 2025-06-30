using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface ILikeService
{
    Task<Result<PostLikeDto>> LikePostAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> UnlikePostAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> IsPostLikedAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Dictionary<Guid,int>> GetPostsLikeCountsAsync(List<Guid> postsIds, CancellationToken ct);
    
    Task<Result<int>> GetPostLikeCountAsync(Guid postId, CancellationToken ct);
    
    Task<Result<int>> GetTotalLikesGivenByUserAsync(Guid userId, CancellationToken ct);

    Task<Result<List<UsernameDto>>> GetUsersWhoLikedPostAsync(Guid postId, CancellationToken ct);
}