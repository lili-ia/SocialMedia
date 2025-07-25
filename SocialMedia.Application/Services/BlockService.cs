using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Application.Services;

public class BlockService : IBlockService
{
    private readonly SocialMediaContext _db;
    private readonly IFollowService _followService;
    private readonly ILogger<BlockService> _logger;
    private readonly IMapper _mapper;
    
    public BlockService(
        SocialMediaContext db, 
        ILogger<BlockService> logger,  
        IMapper mapper, 
        IFollowService followService)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _followService = followService;
    }
    
    public async Task<Result<bool>> BlockUserAsync(Guid blockerId, Guid blockedId, CancellationToken ct)
    {
        var validation = await ValidateUsersAsync(blockerId, blockedId, ct);
       
        if (!validation.Success)
        {
            return validation;
        }
        
        var blockExists = await _db.Blocks
            .AnyAsync(u => u.BlockerId == blockerId && u.BlockedId == blockedId, ct);

        if (blockExists)
        {
            _logger.LogWarning("User with id {BlockerId} already blocked user with id {BlockedId}.", blockerId, blockedId);

            return Result<bool>.FailureResult("You have already blocked this user.", ErrorType.Forbidden);
        }
        
        var isBlockerFollowingResult = await _followService.IsFollowingAsync(blockerId, blockedId, ct);

        if (isBlockerFollowingResult is { Success: true, Value: true })
        {
            var unfollowResult = await _followService.UnfollowUserAsync(blockerId, blockedId, ct);

            if (!unfollowResult.Success)
            {
                _logger.LogWarning("Failed to unfollow user {BlockedId} by blocker {BlockerId} during block process.", blockedId, blockerId);
        
                return Result<bool>.FailureResult("Failed to unfollow user before blocking.", ErrorType.ServerError);
            }
        }

        var isBlockedFollowingResult = await _followService.IsFollowingAsync(blockedId, blockerId, ct);

        if (isBlockedFollowingResult is { Success: true, Value: true })
        {
            var unfollowResult = await _followService.UnfollowUserAsync(blockedId, blockerId, ct);

            if (!unfollowResult.Success)
            {
                _logger.LogWarning("Failed to unfollow user {BlockerId} by blocked {BlockedId} during block process.", blockedId, blockerId);
        
                return Result<bool>.FailureResult("Failed to unfollow user before blocking.", ErrorType.ServerError);
            }
        }
        
        var block = new Block
        {
            BlockerId = blockerId, 
            BlockedId = blockedId, 
            BlockedAt = DateTime.UtcNow
        };
        
        try
        {
            await _db.Blocks.AddAsync(block, ct);
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("User with id {BlockerId} successfully blocked user with id {BlockedId}.", blockerId, blockedId);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogWarning("Duplicate block detected for blocker user {BlockerId} and blocked user {BlockedId}", blockerId, blockedId);
            
            return Result<bool>.FailureResult("You have already blocked this user.", ErrorType.Forbidden);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {BlockerId} trying to block user with id {BlockedId}.", blockerId, blockedId);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<bool>> UnblockUserAsync(Guid blockerId, Guid blockedId, CancellationToken ct)
    {
        var validation = await ValidateUsersAsync(blockerId, blockedId, ct);
        
        if (!validation.Success)
        {
            return validation;
        }
        
        var block = await _db.Blocks
            .FirstOrDefaultAsync(u => u.BlockerId == blockerId && u.BlockedId == blockedId, ct);

        if (block == null)
        {
            _logger.LogWarning("User with id {BlockerId} didn't block user with id {BlockedId}.", blockerId, blockedId);

            return Result<bool>.FailureResult("You didn't block this user.", ErrorType.Forbidden);
        }
        
        try
        {
            _db.Blocks.Remove(block);
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("User with id {BlockerId} successfully unblocked user with id {BlockedId}.", blockerId, blockedId);
            
            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {BlockerId} trying to unblock user with id {BlockedId}.", blockerId, blockedId);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<List<UserPreviewDto>>> GetBlockedUsersAsync(Guid blockerId, CancellationToken ct)
    {
        var blockerUserExists = await UserExistsAsync(blockerId, ct);

        if (!blockerUserExists)
        {
            _logger.LogWarning("Blocker user with ID {BlockerId} not found.", blockerId);

            return Result<List<UserPreviewDto>>.FailureResult("Blocker user not found.", ErrorType.NotFound);
        }

        var blockedUsers = await _db.Blocks
            .AsNoTracking()
            .Where(b => b.BlockerId == blockerId)
            .ProjectTo<UserPreviewDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        
        return Result<List<UserPreviewDto>>.SuccessResult(blockedUsers);
    }

    private async Task<bool> UserExistsAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId, ct);
    }
    
    private async Task<Result<bool>> ValidateUsersAsync(Guid blockerId, Guid blockedId, CancellationToken ct)
    {
        if (blockerId == blockedId)
        {
            return Result<bool>.FailureResult("You can not block or unblock yourself.", ErrorType.Forbidden);
        }

        if (!await UserExistsAsync(blockerId, ct))
        {
            _logger.LogWarning("Blocker user with ID {BlockerId} not found.", blockerId);
            
            return Result<bool>.FailureResult("Blocker user not found.", ErrorType.NotFound);
        }

        if (await UserExistsAsync(blockedId, ct))
        {
            return Result<bool>.SuccessResult(true);
        }
        
        _logger.LogWarning("Blocked user with ID {BlockedId} not found.", blockedId);
        
        return Result<bool>.FailureResult("Blocked user not found.", ErrorType.NotFound);
    }
}