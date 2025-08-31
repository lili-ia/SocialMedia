using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface ICommentRepository
{
    Task AddAsync(Comment comment, CancellationToken cancellationToken = default);

    Task<Comment?> GetByIdWithPostAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> GetAllByPostIdAsync<TResult>(
        Guid postId,
        Expression<Func<Comment, bool>>? predicate,
        Expression<Func<Comment, TResult>> selector,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
}