using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Search;

public class SearchUsersCommandHandler(
    IUserRepository userRepository,
    IBlockRepository blockRepository,
    IFileStorageService storageService)
    : IRequestHandler<SearchUsersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(SearchUsersCommand request, CancellationToken ct)
    {
        IReadOnlyList<Guid>? blockedUsersIds = null;
        
        if (request.ForUserId is not null)
        {
            blockedUsersIds = await blockRepository
                .GetBlockedByEitherIdsAsync(request.ForUserId.Value, ct);
        }

        var searchResult = await userRepository.SearchActiveByUsernameAsync(
            username: request.Username, 
            selector: UserMapper.ToUserPreviewDto,
            excludeIds: blockedUsersIds?.ToList(), 
            ct);
        
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