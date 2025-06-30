using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFeedService
{
    Task<List<PostFeedDto>> GetFeedAsync(
        Guid userId, 
        CancellationToken ct, 
        int page = 1, 
        int pageSize = 20);
    
    Task<List<PostFeedDto>> GetRecentPostsFromUsers(
        List<Guid> followsIds, 
        CancellationToken ct, 
        int page = 1, 
        int pageSize = 20);
    
    Task<List<PostFeedDto>> GetMostPopularPostsAsync(
        List<Guid> excludeUserIds,
        DateTime since,
        int page,
        int pageSize,
        CancellationToken ct);
}
