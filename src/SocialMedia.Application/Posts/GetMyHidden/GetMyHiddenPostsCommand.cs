using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Posts.GetMyHidden;

public sealed record GetMyHiddenPostsCommand(
    Guid UserId,
    int Page, 
    int PageSize) : IRequest<Result<List<PostDto>>>;