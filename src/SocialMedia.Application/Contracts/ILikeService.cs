using SocialMedia.Shared.DTOs.Like;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Contracts;

public interface ILikeService
{
    Task<Result<PostLikeDto>> LikePostAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> UnlikePostAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> IsPostLikedAsync(Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<int>> GetPostLikeCountAsync(Guid postId, CancellationToken ct);
    
    Task<Result<List<UserPreviewDto>>> GetUsersWhoLikedPostAsync(Guid postId, CancellationToken ct);
}