using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;
using SocialMedia.Shared.DTOs;
using SocialMedia.Shared.DTOs.User;

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
        IMapper mapper, 
        IFileStorageService fileStorageService)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
        _fileStorage = fileStorageService;
    }

    public async Task<Result<PrivateUserProfileDto>> UpdateProfileAsync(UpdateUserDto dto, Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .FindAsync([userId], ct);

        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);

            return Result<PrivateUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }

        try
        {
            user.Username = dto.Username;
            user.BirthDate = dto.BirthDate;
            user.Bio = dto.Bio;
            
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("User profile with id {UserId} was successfully updated.", userId);
            
            var updatedDto = _mapper.Map<PrivateUserProfileDto>(user);
        
            return Result<PrivateUserProfileDto>.SuccessResult(updatedDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to update own profile.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<PrivateUserProfileDto>> GetOwnProfileInfoAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectTo<PrivateUserProfileDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (user != null)
        {
            return Result<PrivateUserProfileDto>.SuccessResult(user);
        }
        
        _logger.LogWarning("User with ID {UserId} not found.", userId);
            
        return Result<PrivateUserProfileDto>.FailureResult("User not found.", ErrorType.NotFound);
    }

    public async Task<Result<PrivateUserProfileDto>> UpdateProfilePicAsync(Guid userId, Stream fileStream, string fileName, CancellationToken ct)
    {
        var user = await _db.Users
            .FindAsync([userId], ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("User not found.", ErrorType.NotFound);
        }
        
        try
        {
            await _fileStorage.UploadFileAsync(fileName, fileStream, ct);

            if (!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                var oldFileName = Path.GetFileName(user.ProfilePicUrl);
                await _fileStorage.DeleteFileAsync(oldFileName, ct);
            }

            var blobUrl = $"{Guid.NewGuid()}{_fileStorage.BaseUrl}/{fileName}";
            user.ProfilePicUrl = blobUrl;
            
            await _db.SaveChangesAsync(ct);
            
            _logger.LogInformation("User with id {UserId} successfully updated own profile pic.", userId);
            
            var dto = _mapper.Map<PrivateUserProfileDto>(user);

            return Result<PrivateUserProfileDto>.SuccessResult(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user with id {UserId} trying to update own profile pic.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<PublicUserProfileDto>> GetPublicUserInfoAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectTo<PublicUserProfileDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        if (user != null)
        {
            return Result<PublicUserProfileDto>.SuccessResult(user);
        }
        
        _logger.LogWarning("User with ID {UserId} not found.", userId);
            
        return Result<PublicUserProfileDto>.FailureResult("User not found.", ErrorType.NotFound);
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
            
            return Result<bool>.FailureResult("User not found.", ErrorType.NotFound);
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
            
            _logger.LogInformation("User with id {UserId} was successfully deleted.", userId);
            
            try
            {
                await _fileStorage.DeleteFileAsync(profilePicFileName, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete profile pic file for user {UserId}", userId);
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(ct);
            
            _logger.LogError(e, "An error occurred while trying to delete user with id {UserId}.", userId);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
    }

    public async Task<Result<PagedResult<PublicUserProfileDto>>> SearchUsersAsync(string query, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;
        
        var skip = (pageNumber - 1) * pageSize;
        
        var queryable = _db.Users
            .Where(u => u.Username.ToLower().Contains(query.ToLower()));
        var totalCount = await queryable.CountAsync(ct);

        var users = await queryable
            .ProjectTo<PublicUserProfileDto>(_mapper.ConfigurationProvider)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);


        var pagedResult = new PagedResult<PublicUserProfileDto>
        {
            TotalCount = totalCount,
            Items = users
        };
        
        return Result<PagedResult<PublicUserProfileDto>>.SuccessResult(pagedResult);
    }
}