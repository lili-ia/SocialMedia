using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromPopular;

public class GetFeedFromPopularCommandHandler : IRequestHandler<GetFeedFromPopularCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly IValidator<GetFeedFromPopularCommand> _validator;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetFeedFromPopularCommandHandler> _logger;

    public GetFeedFromPopularCommandHandler(
        IValidator<GetFeedFromPopularCommand> validator, 
        IPostRepository postRepository, 
        ILogger<GetFeedFromPopularCommandHandler> logger)
    {
        _validator = validator;
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetFeedFromPopularCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetFeedFromPopularCommand {@Command}.", request);
        
        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetFeedFromPopularCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }
        
        var skip = (request.Page - 1) * request.PageSize;

        var since = DateTime.UtcNow.AddDays(-7);

        var posts = await _postRepository.GetListAsync(
            predicate: p => p.IsActive 
                            && p.CreatedAt >= since 
                            && p.User.Status == UserStatus.Active,
            selector: PostMapper.ProjectToDto,
            orderBy: q => q
                .OrderByDescending(p => p.PostLikes.Count)
                .ThenByDescending(p => p.PostViews.Count),
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} posts for user {ForUserId}.", posts.Count, request.ForUserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}