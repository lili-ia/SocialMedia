using SocialMedia.Shared.DTOs;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<PrivateUserProfileDto>> UpdateProfileAsync(UpdateUserDto dto, Guid userId, CancellationToken ct);

    Task<Result<PrivateUserProfileDto>> GetOwnProfileInfoAsync(Guid userId, CancellationToken ct);

    Task<Result<PrivateUserProfileDto>> UpdateProfilePicAsync(
        Guid userId, 
        Stream fileStream, 
        string fileName, 
        CancellationToken ct);
    
    Task<Result<PublicUserProfileDto>> GetPublicUserInfoAsync(Guid userId, CancellationToken ct);

    Task<Result<bool>> DeleteUserAsync(Guid userId, CancellationToken ct);

    Task<Result<PagedResult<PublicUserProfileDto>>> SearchUsersAsync(
        string query, 
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default);
}