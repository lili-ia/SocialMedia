using System.Text.Json;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace Infrastructure.Security;

public sealed class BlockCacheService(ICacheService cache, IBlockRepository blockRepository) : IBlockCacheService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    public async Task<IReadOnlySet<Guid>> GetBlockedAndBlockerIdsAsync(Guid userId, CancellationToken ct)
    {
        var key = CacheKey(userId);
        
        var cached = await cache.GetAsync<HashSet<Guid>>(key);

        if (cached is not null)
        {
            return cached;
        }
        
        var blockedTask = blockRepository.GetBlockedIdsAsync(userId, ct);
        var blockersTask = blockRepository.GetBlockerIdsAsync(userId, ct);

        await Task.WhenAll(blockedTask, blockersTask);

        var blocked = blockedTask.Result;
        var blockers = blockersTask.Result;
        
        var combined = new HashSet<Guid>(blocked);
        combined.UnionWith(blockers);

        await cache.SetAsync(key, JsonSerializer.Serialize(combined), Ttl, ct);

        return combined;
    }

    public async Task InvalidateAsync(Guid userId, CancellationToken ct)
        => await cache.RemoveAsync(CacheKey(userId));

    private static string CacheKey(Guid userId) => $"blocks:user:{userId}";
}