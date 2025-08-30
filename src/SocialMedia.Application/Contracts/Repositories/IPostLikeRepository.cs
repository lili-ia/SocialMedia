using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostLikeRepository
{
    Task AddAsync(PostLike postLike, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid likerId, Guid postId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserPreviewDto>> GetPostLikers<TResult>(
        Guid postId, 
        Expression<Func<PostLike, bool>> filter,
        Expression<Func<PostLike, TResult>> selector,
        CancellationToken cancellationToken = default);

    Task<bool> IsLikedByUser(Guid postId, Guid userId, CancellationToken cancellationToken = default);
}