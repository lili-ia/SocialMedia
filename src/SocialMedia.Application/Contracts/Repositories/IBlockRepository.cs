using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IBlockRepository
{
    Task AddAsync(Block block, CancellationToken cancellationToken = default);

    Task<int> RemoveAsync(Guid blockerId, Guid blockedId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> GetUsersBlockedByAsync<TResult>(
        Guid blockerId, 
        Expression<Func<Block, TResult>> selector, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Guid>> GetBlockedIdsAsync(Guid blockerId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Guid>> GetBlockerIdsAsync(Guid blockedId, CancellationToken cancellationToken = default);
}