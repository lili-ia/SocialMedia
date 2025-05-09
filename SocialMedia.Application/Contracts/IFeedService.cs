using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFeedService
{
    Task<List<PostFeedDto>> GetFeedAsync(
        int userId, 
        CancellationToken ct, 
        int page = 1, 
        int pageSize = 20);
    
    Task<List<PostFeedDto>> GetRecentPostsFromUsers(
        List<int> followsIds, 
        CancellationToken ct, 
        int page = 1, 
        int pageSize = 20);
    
    Task<List<PostFeedDto>> GetMostPopularPostsAsync(List<int> excludeUserIds,
        DateTime since,
        int page,
        int pageSize,
        CancellationToken ct);
}
