using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<Post?> GetByIdWithFilesAsync(Guid id, CancellationToken ct = default);
    
    Task<(bool IsActive, Guid AuthorId)?> GetStatusAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAsync(Post post, CancellationToken cancellationToken = default);

    Task<TResult?> GetDetailsAsync<TResult>(
        Guid postId, 
        Expression<Func<Post, TResult>> selector, 
        CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
    
    Task<List<PostDto>> GetPublicOfAuthor(
        Guid authorId,
        Guid? targetUserId,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    
    Task<List<TResult>> GetHiddenOfAuthor<TResult>(
        Guid authorId,
        Expression<Func<Post, TResult>> selector,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default);
    
    Task<Guid?> GetUserIdByPostIdAsync(Guid id, CancellationToken cancellationToken = default);
}