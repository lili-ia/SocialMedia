namespace SocialMedia.Application.Contracts;

public interface IBlockCacheService
{
    Task<IReadOnlySet<Guid>> GetBlockedAndBlockerIdsAsync(Guid userId, CancellationToken ct = default);
}