using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public class GetFeedFromFolloweesCommandHandler : IRequestHandler<GetFeedFromFolloweesCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly IValidator<GetFeedFromFolloweesCommand> _validator;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetFeedFromFolloweesCommandHandler> _logger;
    private readonly IFollowRepository _followRepository;

    public GetFeedFromFolloweesCommandHandler(
        IValidator<GetFeedFromFolloweesCommand> validator, 
        IPostRepository postRepository, 
        ILogger<GetFeedFromFolloweesCommandHandler> logger, 
        IFollowRepository followRepository)
    {
        _validator = validator;
        _postRepository = postRepository;
        _logger = logger;
        _followRepository = followRepository;
    }

    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetFeedFromFolloweesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetFeedFromFolloweesCommand {@Command}.", request);
        
        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetFeedFromFolloweesCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<PostDto>>();
        }

        var followeesIds = await _followRepository
            .GetActiveFolloweesForUserAsync(request.ForUserId, u => u.Id, null, cancellationToken);

        if (followeesIds.Count == 0)
        {
            _logger.LogInformation("User {ForUserId} has no followees.", request.ForUserId);
            
            return Result<IReadOnlyList<PostDto>>.Success([]);
        }
        
        Expression<Func<Post, bool>> filter = p =>
            p.IsActive 
            && followeesIds.Contains(p.UserId) 
            && p.User.Status == UserStatus.Active;
        
        var skip = (request.Page - 1) * request.PageSize;

        var posts = await _postRepository.GetListAsync(
            predicate: filter,
            selector: PostMapper.ProjectToDto,
            orderBy: null,
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} posts for user {ForUserId}.", posts.Count, request.ForUserId);
        
        return Result<IReadOnlyList<PostDto>>.Success(posts);
    }
}
