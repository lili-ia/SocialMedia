using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Users.GetPublicInfo;

public sealed record GetPublicUserInfoCommand(
    Guid UserId, 
    Guid? ForUserId) : IRequest<Result<UserPublicDto>>;