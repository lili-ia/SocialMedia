using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Block;

namespace SocialMedia.Application.Mappers;

public static class BlockMapper
{
    public static Expression<Func<Block, BlockedUserDto>> ProjectToBlockedUserDto =>
        block => new BlockedUserDto
        {
            Id = block.BlockedId,
            Username = block.Blocked.UsernameNormalized,
            ThumbnailProfilePicStorageKey = block.Blocked.CurrentProfilePic.ThumbnailStorageKey,
            BlockedAt = block.CreatedAt
        };
}