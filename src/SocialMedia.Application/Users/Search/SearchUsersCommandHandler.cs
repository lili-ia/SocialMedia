using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Search;

public class SearchUsersCommandHandler : IRequestHandler<SearchUsersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchUsersCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;
    private readonly IValidator<SearchUsersCommand> _validator;

    public SearchUsersCommandHandler(
        IUserRepository userRepository, 
        ILogger<SearchUsersCommandHandler> logger, 
        IBlockRepository blockRepository, 
        IValidator<SearchUsersCommand> validator)
    {
        _userRepository = userRepository;
        _logger = logger;
        _blockRepository = blockRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SearchUsersCommandHandler {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for SearchUsersCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<UserPreviewDto>>();
        }
        
        IReadOnlyList<Guid>? blockedUsersIds = null;
        
        if (request.ForUserId is not null)
        {
            blockedUsersIds = await _blockRepository
                .GetBlockedByEitherIdsAsync(request.ForUserId.Value, cancellationToken);
        }

        var searchResult = await _userRepository.SearchActiveByUsernameAsync(
            username: request.Username, 
            selector: UserMapper.ToUserPreviewDto,
            excludeIds: blockedUsersIds?.ToList(), 
            cancellationToken);

        return Result<IReadOnlyList<UserPreviewDto>>.Success(searchResult);
    }
}