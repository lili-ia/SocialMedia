using System.Linq.Expressions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetMyHidden;

public class GetMyHiddenPostsCommandHandler(
    ILogger<GetMyHiddenPostsCommandHandler> logger,
    IPostRepository postRepository,
    IFileStorageService fileStorage)
    : IRequestHandler<GetMyHiddenPostsCommand, Result<List<PostDto>>>
{
    public async Task<Result<List<PostDto>>> Handle(GetMyHiddenPostsCommand request, CancellationToken ct)
    {
        var skip = (request.Page - 1) * request.PageSize;

        Expression<Func<Post, bool>> belongsToUserAndHidden = p =>
            p.UserId == request.UserId && p.IsHidden;

        Func<IQueryable<Post>, IOrderedQueryable<Post>> orderByCreatedAt = q => q
            .OrderByDescending(p => p.CreatedAt);
        
        var posts = await postRepository.GetListAsync(
            PostMapper.ProjectToDto,
            belongsToUserAndHidden,
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
        
        logger.LogInformation("Retrieved {Count} inactive posts by author {UserId}.", posts.Count, request.UserId);
        
        return Result<List<PostDto>>.Success(posts);
    }
}