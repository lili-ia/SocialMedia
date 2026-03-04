using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetPublicOfUser;

public sealed record GetPublicPostsOfUserCommand: IRequest<Result<IReadOnlyList<PostDto>>>
{
    public Guid? AuthorId { get; }
    
    public string? AuthorUsername { get; }
    
    public Guid? TargetUserId { get; }
    
    public int Page { get; }
    
    public int PageSize { get; }
    
    public string CacheKey => $"user:{AuthorId}:posts";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}