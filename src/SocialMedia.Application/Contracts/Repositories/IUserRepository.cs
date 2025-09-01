using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default);
    
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, UserRole role = UserRole.User, CancellationToken cancellationToken = default);

    Task<bool> IsActiveAsync(Guid userId, UserRole role = UserRole.User, CancellationToken cancellationToken = default);

    Task<Guid?> GetIdByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<string?> GetUsernameByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TResult?> GetActiveDetailsByIdAsync<TResult>(
        Guid id, 
        Expression<Func<User, TResult>> selector,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> SearchActiveByUsernameAsync<TResult>(
        string username, 
        Expression<Func<User, TResult>> selector,
        List<Guid>? excludeIds,
        CancellationToken cancellationToken = default);
}