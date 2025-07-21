using SocialMedia.Application.DTOs.Follow;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Contracts;

public interface IFollowService
{
    Task<Result<FollowDto>> FollowAsync(Guid followerId, Guid followeeId, CancellationToken ct);

    Task<Result<bool>> UnfollowUserAsync(Guid followerId, Guid followeeId, CancellationToken ct);
    
    Task<Result<bool>> IsFollowingAsync(Guid followerId, Guid followeeId, CancellationToken ct);
    
    Task<Result<List<UserPreviewDto>>> GetFollowersAsync(Guid userId, CancellationToken ct);
    
    Task<Result<List<UserPreviewDto>>> GetFolloweesAsync(Guid userId, CancellationToken ct);
}