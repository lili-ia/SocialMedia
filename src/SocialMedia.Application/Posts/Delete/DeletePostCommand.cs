using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Posts.Delete;

public sealed record DeletePostCommand(Guid PostId, Guid UserId) : IRequest<Result>;