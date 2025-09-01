using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Post;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public sealed record GetFeedFromFolloweesCommand(
    Guid ForUserId,
    int Page,
    int PageSize) : IRequest<Result<IReadOnlyList<PostDto>>>;