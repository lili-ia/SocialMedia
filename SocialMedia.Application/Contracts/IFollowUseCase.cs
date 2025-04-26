using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IFollowUseCase
{
    Task<Result<FollowDto>> ExecuteAsync(int followerId, int followeeId, CancellationToken ct);
}