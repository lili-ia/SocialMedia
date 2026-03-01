using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFolloweesOfUser;

// public sealed record GetFolloweesOfUserCommand(
//     Guid UserId, 
//     Guid? ForUserId) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;

public sealed record GetFolloweesOfUserCommand : IRequest<Result<IReadOnlyList<UserPreviewDto>>>
{
    public Guid UserId { get; set; }
    
    public Guid? ForUserId { get; set; }
    
    public int Page { get; init; }
    
    public int PageSize { get; init; }
    
    public string CacheKey => $"user:{ForUserId}:followees:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}