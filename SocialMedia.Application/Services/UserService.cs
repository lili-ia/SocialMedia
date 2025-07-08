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
    
    public UserService(
        SocialMediaContext db, 
        ILogger<UserService> logger, 
        IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
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

    public async Task<Result<PrivateUserProfileDto>> UpdateProfilePic(Guid userId, string filePath, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(userId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            
            return Result<PrivateUserProfileDto>.FailureResult("User not found", ErrorType.NotFound);
        }

        try
        {
            user.ProfilePicUrl = filePath;
            await _db.SaveChangesAsync(ct);
            var dto = _mapper.Map<PrivateUserProfileDto>(user);

            return Result<PrivateUserProfileDto>.SuccessResult(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred in {MethodName}.", nameof(UpdateProfilePic));
            
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
}