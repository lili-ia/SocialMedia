using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<(bool IsActive, Guid AuthorId)?> GetStatusAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAsync(Post post, CancellationToken cancellationToken = default);

    Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<TResult>> GetListAsync<TResult>(
        Expression<Func<Post, TResult>> selector,
        Expression<Func<Post, bool>>? predicate = null,
        Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);
    
    Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken cancellationToken = default);
}