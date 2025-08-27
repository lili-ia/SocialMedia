using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetPublicOfUser;

public sealed record  GetPublicPostsOfUserCommand(
    Guid? AuthorUserId, 
    string? AuthorUsername,
    Guid? TargetUserId, 
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;