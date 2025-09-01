using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFollowersOfUser;

public sealed record GetFollowersOfUserCommand(
    Guid UserId, 
    Guid? ForUserId) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;