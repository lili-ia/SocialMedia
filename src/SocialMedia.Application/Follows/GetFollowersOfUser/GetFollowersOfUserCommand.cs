using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFollowersOfUser;

public sealed record GetFollowersOfUserCommand(
    Guid UserId, 
    Guid? ForUserId, 
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>, ICacheable
{
    public string CacheKey => $"user:{UserId}:followers:for:{ForUserId ?? Guid.Empty}:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}