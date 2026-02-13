using FluentValidation;
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
    IValidator<GetMyHiddenPostsCommand> validator,
    IFileStorageService fileStorage)
    : IRequestHandler<GetMyHiddenPostsCommand, Result<List<PostDto>>>
{
    public async Task<Result<List<PostDto>>> Handle(GetMyHiddenPostsCommand request, CancellationToken ct)
    {
        logger.LogInformation("Handling GetMyInactivePostsCommand {@Command}.", request);
        
        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for GetMyInactivePostsCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<List<PostDto>>();
        }
        
        var skip = (request.Page - 1) * request.PageSize;

        var posts = await postRepository.GetHiddenOfAuthor(
            request.UserId, 
            PostMapper.ProjectToDto,
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