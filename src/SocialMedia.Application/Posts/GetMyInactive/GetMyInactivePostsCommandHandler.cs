using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetMyInactive;

public class GetMyInactivePostsCommandHandler : IRequestHandler<GetMyInactivePostsCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly ILogger<GetMyInactivePostsCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IValidator<GetMyInactivePostsCommand> _validator;

    public GetMyInactivePostsCommandHandler(
        ILogger<GetMyInactivePostsCommandHandler> logger, 
        IPostRepository postRepository, 
        IValidator<GetMyInactivePostsCommand> validator)
    {
        _logger = logger;
        _postRepository = postRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetMyInactivePostsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetMyInactivePostsCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetMyInactivePostsCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }

        var skip = (request.Page - 1) * request.PageSize;
        
        var posts = await _postRepository.GetListAsync(
            selector: PostMapper.ProjectToDto, 
            predicate: p => p.UserId == request.UserId && !p.IsActive, 
            orderBy: q => q
                .OrderByDescending(p => p.CreatedAt),
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} inactive posts by author {UserId}.", 
            posts.Count, request.UserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}