using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetPublicOfUser;

public sealed record GetPublicPostsOfUserCommand(
    Guid? AuthorId, 
    string? AuthorUsername, 
    Guid? TargetUserId, 
    int Page, 
    int PageSize
) : IRequest<Result<IReadOnlyList<PostDto>>>, ICacheable
{
    public string CacheKey => $"user:{AuthorId}:posts";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}