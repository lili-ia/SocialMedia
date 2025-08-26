using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Posts.Delete;

public sealed record DeletePostCommand(Guid PostId, Guid UserId) : IRequest<Result>;