using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Users.GetPublicInfo;

public sealed record GetPublicUserInfoCommand(
    Guid UserId, 
    Guid? ForUserId
) : IRequest<Result<UserPublicDto>>, ICacheable
{
    public string CacheKey => $"users:{UserId}:profile";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}