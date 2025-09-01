using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Follows.GetFolloweesOfUser;

public sealed record GetFolloweesOfUserCommand(
    Guid UserId, 
    Guid? ForUserId) : IRequest<Result<IReadOnlyList<UserPreviewDto>>>;