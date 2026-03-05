using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFolloweesOfUser;

public sealed record GetFolloweesOfUserCommand(
    Guid UserId, 
    Guid? ForUserId, 
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>, ICacheable
{
    public string CacheKey => $"user:{UserId}:followees:for:{ForUserId ?? Guid.Empty}:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}