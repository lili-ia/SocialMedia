using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Block;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Blocks.GetBlockedUsers;

public class GetBlockedUsersCommandHandler : IRequestHandler<GetBlockedUsersCommand, Result<IReadOnlyList<BlockedUserDto>>>
{
    private readonly ILogger<GetBlockedUsersCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;

    public GetBlockedUsersCommandHandler(ILogger<GetBlockedUsersCommandHandler> logger, IBlockRepository blockRepository)
    {
        _logger = logger;
        _blockRepository = blockRepository;
    }

    public async Task<Result<IReadOnlyList<BlockedUserDto>>> Handle(
        GetBlockedUsersCommand request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBlockedUsersCommand {@Command}.", request);

        var blockedUsers = await 
            _blockRepository.GetUsersBlockedByAsync(request.BlockerId, BlockMapper.ProjectToBlockedUserDto, cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} blocked users by user {BlockerId}.", 
            blockedUsers.Count, request.BlockerId);
        
        return Result<IReadOnlyList<BlockedUserDto>>.Success(blockedUsers);
    }
}