using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFeedService
{
    Task<List<PostFeedDto>> GetFeedAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 20,
        CancellationToken ct = default);
    
    Task<List<PostFeedDto>> GetRecentPostsFromUsersAsync(
        List<Guid> followsIds, 
        int fetchCount,
        Guid forUserId,
        CancellationToken ct = default);
    
    Task<List<PostFeedDto>> GetMostPopularPostsSinceDateAsync(
        List<Guid> excludeAuthors,
        DateTime since,
        int fetchCount,
        Guid forUserId,
        CancellationToken ct = default);
}