using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Delete;

public sealed record DeleteFollowCommand(Guid FollowerId, Guid FolloweeId) : IRequest<Result<FollowResponse>>;