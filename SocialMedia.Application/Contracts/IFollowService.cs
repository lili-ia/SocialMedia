using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFollowService
{
    Task<Result<FollowDto>> FollowAsync(int followerId, int followeeId, CancellationToken ct);

    Task<Result<bool>> UnfollowUserAsync(int followerId, int followeeId, CancellationToken ct);
    
    Task<Result<bool>> IsFollowingAsync(int followerId, int followeeId, CancellationToken ct);
    
    Task<IEnumerable<UserDto>> GetFollowersAsync(int userId, CancellationToken ct);
    
    Task<IEnumerable<UserDto>> GetFollowingAsync(int userId, CancellationToken ct);
    
    Task<Result<int>> GetFollowersCountAsync(int userId, CancellationToken ct);
    
    Task<Result<int>> GetFollowingCountAsync(int userId, CancellationToken ct);
}