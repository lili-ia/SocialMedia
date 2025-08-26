using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetPublicByUserId;

public sealed record GetPublicPostsByUserIdCommand(
    Guid AuthorUserId, 
    Guid? TargetUserId, 
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;