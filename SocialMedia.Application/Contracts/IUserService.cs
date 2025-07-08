using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<PrivateUserProfileDto>> UpdateProfileAsync(UpdateUserDto dto, Guid userId, CancellationToken ct);

    Task<Result<PrivateUserProfileDto>> GetOwnProfileInfoAsync(Guid userId, CancellationToken ct);

    Task<Result<PrivateUserProfileDto>> UpdateProfilePic(Guid userId, string filePath, CancellationToken ct);
    
    Task<Result<PublicUserProfileDto>> GetPublicUserInfoAsync(Guid userId, CancellationToken ct);
}