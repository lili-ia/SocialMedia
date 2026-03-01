using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Block;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Blocks.GetBlockedUsers;

public class GetBlockedUsersCommandHandler(
    ILogger<GetBlockedUsersCommandHandler> logger,
    IBlockRepository blockRepository,
    IFileStorageService storageService)
    : IRequestHandler<GetBlockedUsersCommand, Result<IReadOnlyList<BlockedUserDto>>>
{
    public async Task<Result<IReadOnlyList<BlockedUserDto>>> Handle(GetBlockedUsersCommand request, CancellationToken ct)
    {
        var blockedUsers = await 
            blockRepository.GetUsersBlockedByAsync(request.BlockerId, BlockMapper.ProjectToBlockedUserDto, ct);
        
        foreach (var userDto in blockedUsers)
        {
            if (!string.IsNullOrEmpty(userDto.ThumbnailProfilePicStorageKey))
            {
                userDto.ThumbnailProfilePicUrl = storageService.GetPresignedUrl(userDto.ThumbnailProfilePicStorageKey);
            }
        }
        
        logger.LogInformation("Retrieved {Count} blocked users by user {BlockerId}.", blockedUsers.Count, request.BlockerId);
        
        return Result<IReadOnlyList<BlockedUserDto>>.Success(blockedUsers);
    }
}