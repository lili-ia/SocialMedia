using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetMyInactive;

public sealed record GetMyInactivePostsCommand(
    Guid UserId,
    int Page, 
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;