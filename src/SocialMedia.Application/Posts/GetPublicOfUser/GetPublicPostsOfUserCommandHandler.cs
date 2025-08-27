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

public class GetPublicPostsOfUserCommandHandler : IRequestHandler<GetPublicPostsOfUserCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly ILogger<GetPublicPostsOfUserCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IUserBlockChecker _blockChecker;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetPublicPostsOfUserCommand> _validator;
    
    public GetPublicPostsOfUserCommandHandler(
        ILogger<GetPublicPostsOfUserCommandHandler> logger, 
        IPostRepository postRepository, 
        IUserBlockChecker blockChecker, 
        IUserRepository userRepository, 
        IValidator<GetPublicPostsOfUserCommand> validator)
    {
        _logger = logger;
        _postRepository = postRepository;
        _blockChecker = blockChecker;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetPublicPostsOfUserCommand request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Handling GetPublicPostsOfUserCommand {@Command}.", request);
        
        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetPublicPostsOfUserCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }

        var authorId = request.AuthorUserId;
        
        if (request.AuthorUserId is not null)
        {
            var authorExists = await _userRepository.Exists(request.AuthorUserId.Value, UserRole.User, cancellationToken);

            if (!authorExists)
            {
                _logger.LogWarning("Author {AuthorId} not found.", request.AuthorUserId);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        else
        {
            authorId = await _userRepository.GetIdByUsername(request.AuthorUsername!, cancellationToken);
        }
        
        if (authorId is null)
        {
            _logger.LogWarning("Author {AuthorUsername} not found.", request.AuthorUsername);
            
            return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
        }
        
        if (request.TargetUserId is not null)
        {
            var blockExists = await _blockChecker
                .IsBlockedBetweenAsync(authorId.Value, request.TargetUserId.Value, cancellationToken);

            if (blockExists)
            {
                _logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", 
                    request.AuthorUserId, request.TargetUserId.Value);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        
        var skip = (request.Page - 1) * request.PageSize;
        
        var posts = await _postRepository.GetListAsync(
            predicate: p => p.UserId == authorId && p.IsActive, 
            selector: PostMapper.ToDto, 
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} posts by author {AuthorId} for user {TargetUserId}.", 
            posts.Count, request.AuthorUserId, request.TargetUserId?.ToString() ?? "Anonymous");
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}