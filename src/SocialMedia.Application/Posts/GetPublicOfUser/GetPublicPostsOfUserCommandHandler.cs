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

namespace SocialMedia.Application.Posts.GetPublicOfUser;

public class GetPublicPostsOfUserCommandHandler(
    ILogger<GetPublicPostsOfUserCommandHandler> logger,
    IPostRepository postRepository,
    IUserRepository userRepository,
    IFileStorageService fileStorage,
    ICacheService cache,
    IBlockCacheService blockCache)
    : IRequestHandler<GetPublicPostsOfUserCommand, Result<IReadOnlyList<PostDto>>>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
    
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetPublicPostsOfUserCommand request, CancellationToken ct)
    {
        var authorId = request.AuthorUserId;

        var cacheKey = $"posts:user:{authorId}";
        var cachedPosts = await cache.GetAsync<IReadOnlyList<PostDto>>(cacheKey);

        if (cachedPosts is not null)
        {
            return Result<IReadOnlyList<PostDto>>.Success(cachedPosts);
        }
        
        if (authorId is not null)
        {
            var authorExists = await userRepository.ExistsAsync(authorId.Value, UserRole.User, ct);
        
            if (!authorExists)
            {
                logger.LogWarning("Author {AuthorId} not found.", authorId);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        else
        {
            authorId = await userRepository.GetIdByUsernameAsync(request.AuthorUsername!, ct);
        }
        
        if (authorId is null)
        {
            logger.LogWarning("Author {AuthorUsername} not found.", request.AuthorUsername);
            
            return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
        }
        
        if (request.TargetUserId is not null)
        {
            var blockedIds = await blockCache.GetBlockedAndBlockerIdsAsync(request.TargetUserId.Value, ct);

            if (blockedIds.Contains(authorId.Value))
            {
                logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", 
                    request.AuthorUserId, request.TargetUserId.Value);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        
        var skip = (request.Page - 1) * request.PageSize;

        Expression<Func<Post, bool>> mustBelongToUserAndBeActive = p =>
            p.UserId == request.AuthorUserId && !p.IsHidden;

        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderByCreatedAt = q => q
            .OrderByDescending(p => p.CreatedAt);
        
        var posts = await postRepository.GetListAsync(
            PostMapper.ProjectToDto, 
            mustBelongToUserAndBeActive, 
            orderByCreatedAt, 
            skip, 
            request.PageSize, 
            ct);
        
        foreach (var post in posts)
        {
            if (post.FileStorageKeys is not null)
            {
                post.FileUrls = post.FileStorageKeys
                    .Select(key => fileStorage.GetPresignedUrl(key, 60))
                    .ToList();
            }
        }
        
        logger.LogInformation("Retrieved {Count} posts by author {AuthorId} for user {TargetUserId}.", 
            posts.Count, request.AuthorUserId, request.TargetUserId?.ToString() ?? "Anonymous");

        await cache.SetAsync(cacheKey, posts, Ttl, ct);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}