using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Likes.GetPostLikers;

public sealed record GetPostLikersCommand(
    Guid PostId, 
    Guid TargetUserId, 
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>, ICacheable
{
    public string CacheKey => $"post:{PostId}:likers:user:{TargetUserId}:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(5);
}