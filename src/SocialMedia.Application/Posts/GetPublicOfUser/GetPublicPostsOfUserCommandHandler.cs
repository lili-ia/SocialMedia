using Domain.Enums;
using FluentValidation;
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
    IBlockRepository blockRepository,
    IUserRepository userRepository,
    IValidator<GetPublicPostsOfUserCommand> validator,
    IFileStorageService fileStorage)
    : IRequestHandler<GetPublicPostsOfUserCommand, Result<IReadOnlyList<PostDto>>>
{
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetPublicPostsOfUserCommand request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }
        
        var authorId = request.AuthorUserId;
        
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
            var blockExists = await blockRepository
                .IsBlockedByEitherAsync(authorId.Value, request.TargetUserId.Value, ct);
        
            if (blockExists)
            {
                logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", 
                    request.AuthorUserId, request.TargetUserId.Value);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        
        var skip = (request.Page - 1) * request.PageSize;

        var posts = await postRepository.GetPublicOfAuthor(
            authorId.Value,
            request.TargetUserId,
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
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}