using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Feed.GetFromPopular;

public record GetFeedFromPopularCommand(
    Guid ForUserId,
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<PostDto>>>, ICacheable
{
    public string CacheKey => $"feed:popular:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(5);
}