using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default, bool tracking = false);

    Task AddAsync(User user, CancellationToken ct = default); 

    Task<User?> GetByEmailOrUsernameAsync(string email, string username, CancellationToken ct = default, bool tracking = false);
    
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default, bool tracking = false);

    Task<bool> ExistsAsync(Guid userId, UserRole role = UserRole.User, CancellationToken ct = default);

    Task<bool> IsActiveAsync(Guid userId, UserRole role = UserRole.User, CancellationToken ct = default);

    Task<Guid?> GetIdByUsernameAsync(string username, CancellationToken ct = default);

    Task<string?> GetUsernameByIdAsync(Guid id, CancellationToken ct = default);

    Task<TResult?> GetActiveDetailsByIdAsync<TResult>(
        Guid id, 
        Expression<Func<User, TResult>> selector,
        CancellationToken cancellationToken = default);

    Task<List<TResult>> SearchActiveByUsernameAsync<TResult>(
        string username, 
        Expression<Func<User, TResult>> selector,
        List<Guid>? excludeIds,
        CancellationToken cancellationToken = default);
}