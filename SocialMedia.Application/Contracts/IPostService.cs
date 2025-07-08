using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Post>> CreatePost(CreatePostDto postDto, Guid userId, CancellationToken ct);
    
    Task<Result<Post>> GetPost(Guid postId, CancellationToken ct);

    Task<Result<List<Post>>> GetPostsByUserAndActiveStatus(Guid userId, bool isActive, CancellationToken ct);

    Task<Result<Post>> UpdatePost(UpdatePostDto postDto, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeletePost(Guid postId, Guid userId, CancellationToken ct);

    Task<Result<Post>> ChangePostActiveStatus(Guid postId, bool activeStatus, CancellationToken ct);

    Task<Result<List<Post>>> GetPostsOfUsername(string username, CancellationToken ct);
}

