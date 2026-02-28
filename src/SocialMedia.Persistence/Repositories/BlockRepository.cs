using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class BlockRepository(SocialMediaDbContext db) : IBlockRepository
{
    public async Task AddAsync(Block block, CancellationToken cancellationToken = default)
    {
        await db.Blocks.AddAsync(block, cancellationToken);
    }

    public async Task<int> RemoveAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default)
    {
        return await db.Blocks
            .Where(b => b.BlockerId == blockerId && b.BlockedId == blockedId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetUsersBlockedByAsync<TResult>(
        Guid blockerId, 
        Expression<Func<Block, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var blocks = await db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockerId == blockerId)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return blocks.AsReadOnly();
    }
    
    public async Task<IReadOnlyList<Guid>> GetBlockedIdsAsync(Guid blockerId, CancellationToken cancellationToken = default)
    {
        var ids = await db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockerId == blockerId)
            .Select(b => b.BlockedId)
            .ToListAsync(cancellationToken);

        return ids.AsReadOnly();
    }

    public async Task<IReadOnlyList<Guid>> GetBlockerIdsAsync(Guid blockedId, CancellationToken cancellationToken = default)
    {
        var ids = await db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockedId == blockedId)
            .Select(b => b.BlockerId)
            .ToListAsync(cancellationToken);

        return ids.AsReadOnly();
    }
}