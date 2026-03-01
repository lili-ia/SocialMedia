using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Feed.GetFromPopular;

public record GetFeedFromPopularCommand : IRequest<Result<IReadOnlyList<PostDto>>>, ICacheable
{
    public int Page { get; init; }
    
    public int PageSize { get; init; }
    
    public Guid ForUserId { get; init; }

    public string CacheKey => $"feed:popular:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(5);
}