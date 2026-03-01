using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default, bool tracking = false);
    
    Task<Post?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(Post post, CancellationToken cancellationToken = default);

    Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default);
    
    Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Post?> GetByIdWithAuthorAsync(Guid id, CancellationToken ct = default);

    Task<List<TResult>> GetListAsync<TResult>(
        Expression<Func<Post, TResult>> selector,
        Expression<Func<Post, bool>> predicate,
        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderBy,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
}