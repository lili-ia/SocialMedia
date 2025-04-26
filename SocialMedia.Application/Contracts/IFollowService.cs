using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface IFollowService
{
    Task<Follow?> FollowAsync(int followerId, int followeeId, CancellationToken ct);
}