using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Users.GetPrivateInfo;

public sealed record GetPrivateUserInfoCommand(Guid UserId) : IRequest<Result<UserPrivateDto>>;