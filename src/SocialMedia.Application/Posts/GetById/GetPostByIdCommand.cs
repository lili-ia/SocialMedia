using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetById;

public sealed record GetPostByIdCommand(
    Guid PostId, 
    Guid? TargetUserId
) : IRequest<Result<PostDto>>, ICacheable
{
    public string CacheKey => $"posts:{PostId}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}