using Infrastructure.Models;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Post>> CreatePost(CreatePostDto postDto, int userId);
    
    Task<Result<Post>> GetPost(int postId);

    Task<Result<List<Post>>> GetPostsByUserAndActiveStatus(int userId, bool isActive);

    Task<Result<Post>> UpdatePost(UpdatePostDto postDto, int postId, int userId);
    
    Task<Result<bool>> DeletePost(int postId, int userId);

    Task<Result<Post>> ChangePostActiveStatus(int postId, bool activeStatus);

    Task<Result<List<Post>>> GetPostsOfUsername(string username);
}

