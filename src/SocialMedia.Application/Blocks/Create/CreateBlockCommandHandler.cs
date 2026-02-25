using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.Exceptions;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Block;

namespace SocialMedia.Application.Blocks.Create;

public class CreateBlockCommandHandler(
    ILogger<CreateBlockCommandHandler> logger,
    IBlockRepository blockRepository,
    IFollowRepository followRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<CreateBlockCommand, Result<BlockResponse>>
{
    public async Task<Result<BlockResponse>> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
    {
        var blockedUserExists = await userRepository.ExistsAsync(request.BlockedId, UserRole.User, cancellationToken);
        
        if (!blockedUserExists)
        {
            logger.LogWarning("Blocked user {UserId} not found.", request.BlockedId);
            
            return Result<BlockResponse>.Failure("User not found.", ErrorType.NotFound);
        }

        var block = Block.Create(
            request.BlockerId,
            request.BlockedId);

        try
        {
            await followRepository.RemoveMutualAsync(request.BlockerId, request.BlockedId, cancellationToken);
            await blockRepository.AddAsync(block, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {BlockerId} successfully blocked user {BlockedId}.", request.BlockerId, request.BlockedId);

            var dto = new BlockResponse
            {
                BlockerId = request.BlockerId,
                BlockedId = request.BlockedId,
                BlockedAt = block.CreatedAt
            };
            
            return Result<BlockResponse>.Success(dto);
        }
        catch (DuplicateBlockException)
        {
            logger.LogInformation("User {BlockerId} already blocked user {BlockedId}.", request.BlockerId, request.BlockedId);
            
            return Result<BlockResponse>.Failure("You already liked this post.", ErrorType.Conflict);
        }
    }
}