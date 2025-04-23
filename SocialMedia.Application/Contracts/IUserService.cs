using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<User>> UpdateProfileAsync(UpdateUserDto dto, int userId, CancellationToken cancellationToken);

    Task<Result<UserProfileDto>> GetUserInfoAsync(int userId, CancellationToken cancellationToken);

    Task<Result<UserProfileDto>> UpdateProfilePic(int userId, string filePath, CancellationToken ct);
}