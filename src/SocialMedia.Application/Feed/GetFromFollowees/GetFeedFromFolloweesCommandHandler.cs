using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public class GetFeedFromFolloweesCommandHandler(
    ILogger<GetFeedFromFolloweesCommandHandler> logger,
    IFollowRepository followRepository,
    IPostRepository postRepository)
    : IRequestHandler<GetFeedFromFolloweesCommand, Result<IReadOnlyList<PostDto>>>
{
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetFeedFromFolloweesCommand request, CancellationToken ct)
    {
        var followeesIds = await followRepository
            .GetActiveFolloweesForUserAsync(
                userId: request.ForUserId, 
                selector: f => f.FolloweeId, 
                excludeIds: null, 
                ct);

        if (followeesIds.Count == 0)
        {
            logger.LogInformation("User {ForUserId} has no followees.", request.ForUserId);
            
            return Result<IReadOnlyList<PostDto>>.Success([]);
        }
        
        Expression<Func<Post, bool>> filter = p =>
            !p.IsHidden 
            && followeesIds.Contains(p.UserId) 
            && p.User.Status == UserStatus.Active;

        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderByCreatedAt = q => q
            .OrderByDescending(p => p.CreatedAt);
        
        var skip = (request.Page - 1) * request.PageSize;

        var posts = await postRepository.GetListAsync(
            selector: PostMapper.ProjectToDto,
            predicate: filter,
            orderBy: orderByCreatedAt,
            skip: skip,
            take: request.PageSize,
            ct);
        
        logger.LogInformation("Retrieved {Count} posts for user {ForUserId}.", posts.Count, request.ForUserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}
