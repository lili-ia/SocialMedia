using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Users.GetPublicInfo;

public sealed record GetPublicUserInfoCommand : IRequest<Result<UserPublicDto>>
{
    public Guid UserId { get; }
    public Guid? ForUserId { get; }
    
    public string CacheKey => $"users:{UserId}:profile";

    public TimeSpan Ttl => TimeSpan.FromMinutes(10);
}