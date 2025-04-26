using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface ILikePostUseCase
{
    Task<Result<PostLikeDto>> ExecuteAsync(int postId, int userId, CancellationToken ct);
}