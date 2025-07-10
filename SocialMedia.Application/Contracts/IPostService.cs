using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Post>> CreatePostAsync(CreatePostDto postDto, Guid userId, CancellationToken ct);
    
    Task<Result<Post>> GetPostByIdAsync(Guid postId, CancellationToken ct);

    Task<Result<List<Post>>> GetPostsByUserAndActiveStatusAsync(Guid userId, bool isActive, CancellationToken ct);

    Task<Result<Post>> UpdatePostAsync(UpdatePostDto postDto, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId, CancellationToken ct);

    Task<Result<Post>> ChangePostActiveStatusAsync(Guid postId, bool activeStatus, CancellationToken ct);

    Task<Result<List<Post>>> GetPostsOfUsernameAsync(string username, CancellationToken ct);
}

