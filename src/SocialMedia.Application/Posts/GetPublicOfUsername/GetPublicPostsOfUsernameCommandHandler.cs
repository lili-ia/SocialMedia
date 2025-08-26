using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetPublicOfUsername;

public class GetPublicPostsOfUsernameCommandHandler 
    : IRequestHandler<GetPublicPostsOfUsernameCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly ILogger<GetPublicPostsOfUsernameCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IUserBlockChecker _blockChecker;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<GetPublicPostsOfUsernameCommand> _validator;

    public GetPublicPostsOfUsernameCommandHandler(
        ILogger<GetPublicPostsOfUsernameCommandHandler> logger, 
        IPostRepository postRepository, 
        IUserBlockChecker blockChecker, 
        IUserRepository userRepository, 
        IValidator<GetPublicPostsOfUsernameCommand> validator)
    {
        _logger = logger;
        _postRepository = postRepository;
        _blockChecker = blockChecker;
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetPublicPostsOfUsernameCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPublicPostsOfUsernameCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetPublicPostsOfUsernameCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }
        
        var authorId = await _userRepository.GetIdByUsername(request.AuthorUsername, cancellationToken);

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
                    authorId.Value, request.TargetUserId.Value);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        
        var skip = (request.Page - 1) * request.PageSize;
        
        var posts = await _postRepository.GetListAsync(
            predicate: p => p.UserId == authorId.Value && p.IsActive, 
            selector: PostMapper.ToDto, 
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} posts by author {AuthorUsername} for user {TargetUserId}.", 
            posts.Count, request.AuthorUsername, request.TargetUserId?.ToString() ?? "Anonymous");
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}