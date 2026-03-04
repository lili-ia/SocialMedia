using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Likes.GetPostLikers;

public sealed record GetPostLikersCommand : IRequest<Result<IReadOnlyList<UserPreviewDto>>>
{
    public Guid PostId { get; set; }
    
    public Guid TargetUserId { get; set; } 
    
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public string CacheKey => $"post:{PostId}:likers:page:{Page}:size:{PageSize}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(5);
}