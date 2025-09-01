using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Feed.GetFromPopular;

public sealed record GetFeedFromPopularCommand(
    Guid ForUserId,
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;