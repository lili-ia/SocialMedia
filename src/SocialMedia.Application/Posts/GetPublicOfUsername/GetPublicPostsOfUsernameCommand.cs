using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetPublicOfUsername;

public sealed record GetPublicPostsOfUsernameCommand(
    string AuthorUsername, 
    Guid? TargetUserId, 
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;