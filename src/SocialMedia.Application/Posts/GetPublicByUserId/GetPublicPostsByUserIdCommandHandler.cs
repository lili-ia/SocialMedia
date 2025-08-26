using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetPublicByUserId;

public class GetPublicPostsByUserIdCommandHandler : IRequestHandler<GetPublicPostsByUserIdCommand, Result<IReadOnlyList<PostDto>>>
{
    private readonly ILogger<GetPublicPostsByUserIdCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IUserBlockChecker _blockChecker;
    
    public GetPublicPostsByUserIdCommandHandler(
        ILogger<GetPublicPostsByUserIdCommandHandler> logger, 
        IPostRepository postRepository, 
        IUserBlockChecker blockChecker)
    {
        _logger = logger;
        _postRepository = postRepository;
        _blockChecker = blockChecker;
    }
    
    public async Task<Result<IReadOnlyList<PostDto>>> Handle(GetPublicPostsByUserIdCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPublicPostsByUserIdCommand {@Command}.", request);
        
        if (request.TargetUserId is not null)
        {
            var blockExists = await _blockChecker
                .IsBlockedBetweenAsync(request.AuthorUserId, request.TargetUserId.Value, cancellationToken);

            if (blockExists)
            {
                _logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", 
                    request.AuthorUserId, request.TargetUserId.Value);
            
                return Result<IReadOnlyList<PostDto>>.Failure("Author not found.", ErrorType.NotFound);
            }
        }
        
        var skip = (request.Page - 1) * request.PageSize;
        
        var posts = await _postRepository.GetListAsync(
            predicate: p => p.UserId == request.AuthorUserId && p.IsActive, 
            selector: PostMapper.ToDto, 
            skip: skip,
            take: request.PageSize,
            cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} posts by author {AuthorId} for user {TargetUserId}.", 
            posts.Count, request.AuthorUserId, request.TargetUserId?.ToString() ?? "Anonymous");
        
        return Result<IReadOnlyList<PostDto>>.Success(posts.ToList());
    }
}