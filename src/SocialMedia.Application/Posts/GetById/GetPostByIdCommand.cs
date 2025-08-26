using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetById;

public sealed record GetPostByIdCommand(Guid PostId, Guid? TargetUserId) : IRequest<Result<PostDto>>;