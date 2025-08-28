using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Block;

namespace SocialMedia.Application.Blocks.GetBlockedUsers;

public sealed record GetBlockedUsersCommand(Guid BlockerId) : IRequest<Result<IReadOnlyList<BlockedUserDto>>>;