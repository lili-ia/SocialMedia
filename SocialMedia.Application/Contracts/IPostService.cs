using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Post>> CreatePost(CreatePostDto postDto, int userId, CancellationToken cancellationToken);
    
    Task<Result<Post>> GetPost(int postId, CancellationToken cancellationToken);

    Task<Result<List<Post>>> GetPostsByUserAndActiveStatus(int userId, bool isActive, CancellationToken cancellationToken);

    Task<Result<Post>> UpdatePost(UpdatePostDto postDto, int postId, int userId, CancellationToken cancellationToken);
    
    Task<Result<bool>> DeletePost(int postId, int userId, CancellationToken cancellationToken);

    Task<Result<Post>> ChangePostActiveStatus(int postId, bool activeStatus, CancellationToken cancellationToken);

    Task<Result<List<Post>>> GetPostsOfUsername(string username, CancellationToken cancellationToken);
}

