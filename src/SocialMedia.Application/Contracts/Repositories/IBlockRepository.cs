using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IBlockRepository
{
    Task<bool> ExistsAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default);

    Task AddAsync(Block block, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> GetUsersBlockedByAsync<TResult>(
        Guid blockerId, 
        Expression<Func<Block, TResult>> selector, 
        CancellationToken cancellationToken = default);
    
    Task<bool> IsBlockedByEitherAsync(Guid userId, Guid otherUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetBlockedByEitherIdsAsync(Guid userId, CancellationToken cancellationToken = default);
}