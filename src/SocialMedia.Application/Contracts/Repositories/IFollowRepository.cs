using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IFollowRepository
{
    Task RemoveMutualAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);

    Task AddAsync(Follow follow, CancellationToken cancellationToken = default);

    Task<int> GetActiveFollowerCountForUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid followerId, Guid followeeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> GetActiveFolloweesForUserAsync<TResult>(
        Guid userId, 
        Expression<Func<Follow, TResult>> selector,
        IList<Guid>? excludeIds,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<TResult>> GetActiveFollowersForUserAsync<TResult>(
        Guid userId, 
        Expression<Func<Follow, TResult>> selector,
        IList<Guid>? excludeIds,
        CancellationToken cancellationToken = default);
}