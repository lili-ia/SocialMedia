using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFollowService
{
    Task<Result<FollowDto>> FollowAsync(Guid followerId, Guid followeeId, CancellationToken ct);

    Task<Result<bool>> UnfollowUserAsync(Guid followerId, Guid followeeId, CancellationToken ct);
    
    Task<Result<bool>> IsFollowingAsync(Guid followerId, Guid followeeId, CancellationToken ct);
    
    Task<IEnumerable<UserDto>> GetFollowersAsync(Guid userId, CancellationToken ct);
    
    Task<IEnumerable<UserDto>> GetFollowingAsync(Guid userId, CancellationToken ct);
    
    Task<Result<int>> GetFollowersCountAsync(Guid userId, CancellationToken ct);
    
    Task<Result<int>> GetFollowingCountAsync(Guid userId, CancellationToken ct);
}