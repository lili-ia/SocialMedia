using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.Search;

public class SearchUsersCommandHandler(
    IUserRepository userRepository,
    IBlockCacheService blockCache,
    IFileStorageService storageService)
    : IRequestHandler<SearchUsersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(SearchUsersCommand request, CancellationToken ct)
    {
        IReadOnlyList<Guid>? blockedUsersIds = null;
        
        if (request.ForUserId is not null)
        {
            var blockedSet = await blockCache.GetBlockedAndBlockerIdsAsync(request.ForUserId.Value, ct);
            blockedUsersIds = blockedSet.ToList();
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