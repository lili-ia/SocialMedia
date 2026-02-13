using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.Create;

public sealed record CreatePostCommand(
    Guid UserId,
    string? Text, 
    List<FileData>? Files) : IRequest<Result<PostDto>>;