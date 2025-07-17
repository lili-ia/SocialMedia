using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Contracts;

public interface IPostService
{
    Task<Result<Guid>> CreatePostAsync(CreatePostRequest request, Guid userId, CancellationToken ct);
    
    Task<Result<PostDto>> GetPostByIdAsync(Guid postId, Guid forUserId, CancellationToken ct);

    Task<Result<List<PostDto>>> GetPublicPostsByUserId(Guid userId, CancellationToken ct);
    
    Task<Result<List<PostDto>>> GetMyInactivePosts(Guid userId, CancellationToken ct);

    Task<Result<PostDto>> UpdatePostAsync(UpdatePostDto updatePostDto, Guid postId, Guid userId, CancellationToken ct);
    
    Task<Result<bool>> DeletePostAsync(Guid postId, Guid userId, CancellationToken ct);

    Task<Result<bool>> ChangePostActiveStatusAsync(Guid postId, bool activeStatus, CancellationToken ct);

    Task<Result<List<PostDto>>> GetPostsOfUsernameAsync(string username, int page = 1, int pageSize = 20, CancellationToken ct = default);
}

