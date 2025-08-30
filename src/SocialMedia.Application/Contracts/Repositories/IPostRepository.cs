using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task AddAsync(Post post, CancellationToken cancellationToken = default);

    Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default);

    Task RemoveAsync(Post post, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<TResult>> GetListAsync<TResult>(
        Expression<Func<Post, bool>>? predicate,
        Expression<Func<Post, TResult>> selector,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> GetUserIdByPostId(Guid id, CancellationToken cancellationToken = default);
}