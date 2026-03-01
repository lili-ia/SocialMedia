namespace SocialMedia.Application.Contracts;

public interface IBlockCacheService
{
    Task<IReadOnlySet<Guid>> GetBlockedAndBlockerIdsAsync(Guid userId, CancellationToken ct = default);

    Task InvalidateAsync(Guid userId, CancellationToken ct);
}