using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Post>> CreatePost(CreatePostDto postDto, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<Post>> GetPost(Guid postId, CancellationToken cancellationToken);

    Task<Result<List<Post>>> GetPostsByUserAndActiveStatus(Guid userId, bool isActive, CancellationToken cancellationToken);

    Task<Result<Post>> UpdatePost(UpdatePostDto postDto, Guid postId, Guid userId, CancellationToken cancellationToken);
    
    Task<Result<bool>> DeletePost(Guid postId, Guid userId, CancellationToken cancellationToken);

    Task<Result<Post>> ChangePostActiveStatus(Guid postId, bool activeStatus, CancellationToken cancellationToken);

    Task<Result<List<Post>>> GetPostsOfUsername(string username, CancellationToken cancellationToken);
}

