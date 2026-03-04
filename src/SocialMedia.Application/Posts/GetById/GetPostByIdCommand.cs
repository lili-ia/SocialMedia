using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetById;

public sealed record GetPostByIdCommand : IRequest<Result<PostDto>>
{
    public Guid PostId { get; }
    
    public Guid? TargetUserId { get; }
    
    public string CacheKey => $"posts:{PostId}";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}