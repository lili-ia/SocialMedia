using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Follows.Delete;

public sealed record DeleteFollowCommand(Guid FollowerId, Guid FolloweeId) : IRequest<Result>;