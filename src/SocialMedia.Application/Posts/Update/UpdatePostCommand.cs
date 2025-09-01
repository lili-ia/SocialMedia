using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.Update;

public sealed record UpdatePostCommand(Guid PostId, Guid UserId, string Text) : IRequest<Result<PostDto>>;