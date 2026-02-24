using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostLikeRepository
{
    Task AddAsync(PostLike postLike, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default);

    Task<int> RemoveAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResult>> GetNotBlockedPostLikersAsync<TResult>(
        Guid postId, 
        Guid targetUserId,
        Expression<Func<PostLike, TResult>> selector,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<bool> IsLikedByUserAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);

    Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken = default);
}