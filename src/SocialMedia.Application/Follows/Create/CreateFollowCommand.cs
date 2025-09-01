using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Create;

public sealed record CreateFollowCommand(
    Guid FollowerId,
    Guid FolloweeId) : IRequest<Result<FollowResponse>>;