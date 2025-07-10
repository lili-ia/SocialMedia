using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class UserService : IUserService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;
    
    public UserService(
        SocialMediaContext db, 
        ILogger<UserService> logger, 
        IMapper mapper, IFileStorageService fileStorageService)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _fileStorage = fileStorageService;
    }

    public async Task<Result<PrivateUserProfileDto>> UpdateProfileAsync(UpdateUserDto dto, Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(userId, ct);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);

            return Result<PrivateUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }

        try
        {
            _mapper.Map(dto, user);
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred in {MethodName}.", nameof(UpdateProfileAsync));
            
            return Result<PrivateUserProfileDto>.FailureResult(
                "An error occured while updating user info.", ErrorType.ServerError);
        }

        var updatedDto = _mapper.Map<PrivateUserProfileDto>(user);
        
        return Result<PrivateUserProfileDto>.SuccessResult(updatedDto);
    }

    public async Task<Result<PrivateUserProfileDto>> GetOwnProfileInfoAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }

        var userDto = _mapper.Map<PrivateUserProfileDto>(user);
        
        return Result<PrivateUserProfileDto>.SuccessResult(userDto);
    }

    public async Task<Result<PrivateUserProfileDto>> UpdateProfilePicAsync(
        Guid userId, 
        Stream fileStream,
        string fileName, 
        CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(userId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }
        
        try
        {
            await _fileStorage.UploadFileAsync(fileName, fileStream, ct);

            if (!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                var oldFileName = Path.GetFileName(user.ProfilePicUrl);
                await _fileStorage.DeleteFileAsync(oldFileName, ct);
            }

            var blobUrl = $"{_fileStorage.BaseUrl}/{fileName}";
            user.ProfilePicUrl = blobUrl;
            
            await _db.SaveChangesAsync(ct);
            var dto = _mapper.Map<PrivateUserProfileDto>(user);

            return Result<PrivateUserProfileDto>.SuccessResult(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred in {MethodName}.", nameof(UpdateProfilePicAsync));
            
            return Result<PrivateUserProfileDto>.FailureResult(
                $"An error occured while updating user profile pic.", ErrorType.ServerError);
        }
    }

    public async Task<Result<PublicUserProfileDto>> GetPublicUserInfoAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PublicUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }

        var dto = _mapper.Map<PublicUserProfileDto>(user);
        
        return Result<PublicUserProfileDto>.SuccessResult(dto);
    }

    public async Task<Result<bool>> DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.Posts)
            .Include(u => u.Followees)
            .Include(u => u.Followers)
            .Include(u => u.Notifications)
            .Include(u => u.PostLikes)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
   
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            return Result<bool>.FailureResult("User not found", ErrorType.NotFound);
        }

        var profilePicFileName = Path.GetFileName(user.ProfilePicUrl);

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        
        try
        {
            _db.Posts.RemoveRange(user.Posts);
            _db.Follows.RemoveRange(user.Followees);
            _db.Follows.RemoveRange(user.Followers);
            _db.Notifications.RemoveRange(user.Notifications);
            _db.PostLikes.RemoveRange(user.PostLikes);
            _db.RefreshTokens.RemoveRange(user.RefreshTokens);
            _db.Users.Remove(user);
            
            await _db.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
            
            if (!string.IsNullOrWhiteSpace(profilePicFileName))
            {
                await _fileStorage.DeleteFileAsync(profilePicFileName, ct);
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(ex, "Error occurred while deleting user {UserId}", userId);
            return Result<bool>.FailureResult("An error occurred while deleting the user.", ErrorType.ServerError);
        }
    }
}