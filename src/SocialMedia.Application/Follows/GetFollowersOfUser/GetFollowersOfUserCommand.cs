using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFollowersOfUser;

public sealed record GetFollowersOfUserCommand : IRequest<Result<IReadOnlyList<UserPreviewDto>>>
{
    public Guid UserId { get; set; }
    
    public Guid? ForUserId { get; set; }
    
    public int Page { get; init; }
    
    public int PageSize { get; init; }
    
    public string CacheKey => $"user:{ForUserId}:followers:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}