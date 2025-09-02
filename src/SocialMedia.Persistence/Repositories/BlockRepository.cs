using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class BlockRepository : IBlockRepository
{
    private readonly SocialMediaDbContext _db;
    
    public BlockRepository(SocialMediaDbContext db)
    {
        _db = db;
    }
    
    public Task<bool> ExistsAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default)
    {
        return _db.Blocks.AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId, cancellationToken);
    }

    public async Task AddAsync(Block block, CancellationToken cancellationToken = default)
    {
        await _db.Blocks.AddAsync(block, cancellationToken);
    }

    public async Task RemoveAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default)
    {
        await _db.Blocks
            .Where(b => b.BlockerId == blockerId && b.BlockedId == blockedId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> GetUsersBlockedByAsync<TResult>(
        Guid blockerId, 
        Expression<Func<Block, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var blocks = await _db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockerId == blockerId)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return blocks.AsReadOnly();
    }

    public async Task<bool> IsBlockedByEitherAsync(
        Guid userId, 
        Guid otherUserId, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Blocks
            .AnyAsync(b =>
                b.BlockerId == userId && b.BlockedId == otherUserId ||
                b.BlockerId == otherUserId && b.BlockedId == userId, 
                cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetBlockedByEitherIdsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var ids = await _db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .ToListAsync(cancellationToken);

        return ids.AsReadOnly();
    }
}