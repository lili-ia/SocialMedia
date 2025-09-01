using System.Linq.Expressions;
using Domain.Entities;
using SocialMedia.Application.DTOs.Block;

namespace SocialMedia.Application.Mappers;

public static class BlockMapper
{
    public static Expression<Func<Block, BlockedUserDto>> ProjectToBlockedUserDto =>
        block => new BlockedUserDto
        {
            BlockedUserId = block.Blocked.Id,
            BlockedUsername = block.Blocked.Status != Domain.Enums.UserStatus.Deactivated
                ? block.Blocked.Username 
                : "deleted",
            BlockedAt = block.BlockedAt
        };
}