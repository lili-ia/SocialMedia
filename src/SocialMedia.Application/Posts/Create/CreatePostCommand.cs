using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Posts.Create;

public sealed record CreatePostCommand(
    Guid UserId,
    string? Text, 
    List<FileData>? Files) : IRequest<Result<Guid>>;