using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromPopular;

public class GetFeedFromPopularCommandHandler(
    IPostRepository postRepository,
    ILogger<GetFeedFromPopularCommandHandler> logger,
    IBlockCacheService blockCacheService)
    : IRequestHandler<GetFeedFromPopularCommand, Result<IReadOnlyList<PostDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
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
        
        var notShowFrom = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.ForUserId, ct);
        var result = posts.Where(p => !notShowFrom.Contains(p.UserId)).ToList();

        logger.LogInformation("Cache miss for popular feed page {Page}. Retrieved {Count} posts for user {UserId}.",
            request.Page, result.Count, request.ForUserId);

        return Result<IReadOnlyList<PostDto>>.Success(result);
    }
}