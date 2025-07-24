using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Contracts;

public interface IBlockService
{
    Task<Result<bool>> BlockUserAsync(Guid blockerId, Guid blockedId, CancellationToken ct);

    Task<Result<bool>> UnblockUserAsync(Guid blockerId, Guid blockedId, CancellationToken ct);

    Task<Result<bool>> IsBlockedAsync(Guid blockerId, Guid blockedId, CancellationToken ct);

    Task<Result<List<UserPreviewDto>>> GetBlockedUsersAsync(Guid blockerId, CancellationToken ct);
}