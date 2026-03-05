using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public record GetFeedFromFolloweesCommand(
    Guid ForUserId,
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<PostDto>>>, ICacheable
{
    public string CacheKey => $"feed:followees:user:{ForUserId}:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(5);
}