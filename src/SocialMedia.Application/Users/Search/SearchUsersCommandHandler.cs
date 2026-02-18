using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Search;

public class SearchUsersCommandHandler(
    IUserRepository userRepository,
    ILogger<SearchUsersCommandHandler> logger,
    IBlockRepository blockRepository,
    IValidator<SearchUsersCommand> validator,
    IFileStorageService storageService)
    : IRequestHandler<SearchUsersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(SearchUsersCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for SearchUsersCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<UserPreviewDto>>();
        }
        
        IReadOnlyList<Guid>? blockedUsersIds = null;
        
        if (request.ForUserId is not null)
        {
            blockedUsersIds = await blockRepository
                .GetBlockedByEitherIdsAsync(request.ForUserId.Value, cancellationToken);
        }

        var searchResult = await userRepository.SearchActiveByUsernameAsync(
            username: request.Username, 
            selector: UserMapper.ToUserPreviewDto,
            excludeIds: blockedUsersIds?.ToList(), 
            cancellationToken);
        
        foreach (var userDto in searchResult)
        {
            if (!string.IsNullOrEmpty(userDto.ThumbnailProfilePicStorageKey))
            {
                userDto.ThumbnailProfilePicUrl = storageService.GetPresignedUrl(userDto.ThumbnailProfilePicStorageKey);
            }
        }

        return Result<IReadOnlyList<UserPreviewDto>>.Success(searchResult);
    }
}