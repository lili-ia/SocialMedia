using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromPopular;

public class GetFeedFromPopularCommandHandler(
    IPostRepository postRepository,
    ILogger<GetFeedFromPopularCommandHandler> logger)
    : IRequestHandler<GetFeedFromPopularCommand, Result<IReadOnlyList<PostDto>>>
{

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetFeedFromPopularCommand request, CancellationToken ct)
    {
        var skip = (request.Page - 1) * request.PageSize;

        var since = DateTime.UtcNow.AddDays(-7);
        
        Expression<Func<Post, bool>> mustBeActiveAndNew = p =>
            !p.IsHidden &&
            p.CreatedAt >= since &&
            p.User.Status == UserStatus.Active;

        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderByLikesThenViews = q => q
            .OrderByDescending(p => p.LikeCount)
            .ThenByDescending(p => p.ViewCount);
        
        var posts = await postRepository.GetListAsync<PostDto>(
            selector: PostMapper.ProjectToDto,
            predicate: mustBeActiveAndNew,
            orderBy: orderByLikesThenViews,
            skip: skip,
            take: request.PageSize,
            ct);
        
        logger.LogInformation("Retrieved {Count} posts for user {ForUserId}.", posts.Count, request.ForUserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}