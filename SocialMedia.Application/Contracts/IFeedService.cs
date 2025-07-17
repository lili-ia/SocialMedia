using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Contracts;

public interface IFeedService
{
    Task<List<PostDto>> GetFeedAsync(
        Guid userId, 
        int page = 1, 
        int pageSize = 20,
        CancellationToken ct = default);
    
    Task<List<PostDto>> GetRecentPostsFromUsersAsync(
        List<Guid> followsIds, 
        int fetchCount,
        Guid forUserId,
        CancellationToken ct = default);
    
    Task<List<PostDto>> GetMostPopularPostsSinceDateAsync(
        List<Guid> excludeAuthors,
        DateTime since,
        int fetchCount,
        Guid forUserId,
        CancellationToken ct = default);
}