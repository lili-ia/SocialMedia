using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class UserService : IUserService
{
    private readonly SocialMediaContext _db;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    
    public UserService(
        SocialMediaContext db, 
        IPasswordService passwordService,
        ILogger<UserService> logger, 
        IJwtService jwtService,
        IMapper mapper)
    {
        _db = db;
        _passwordService = passwordService;
        _logger = logger;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<Result<User>> UpdateProfileAsync(UpdateUserDto dto, int userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(userId);

        if (user == null)
        {
            return Result<User>.FailureResult($"Couldn`t find a user with id {userId}", ErrorType.NotFound);
        }

        try
        {
            _mapper.Map(dto, user);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return Result<User>.FailureResult(
                $"An error occured while updating user info.", ErrorType.ServerError);
        }

        return Result<User>.SuccessResult(user);
    }

    public async Task<Result<UserProfileDto>> GetUserInfoAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(userId, cancellationToken);
        
        if (user == null)
        {
            return Result<UserProfileDto>.FailureResult($"Couldn`t find a user with id {userId}", ErrorType.NotFound);
        }

        var userDto = _mapper.Map<UserProfileDto>(user);
        
        return Result<UserProfileDto>.SuccessResult(userDto);
    }

    public async Task<Result<UserProfileDto>> UpdateProfilePic(int userId, string filePath, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(userId, ct);
        
        if (user == null)
        {
            return Result<UserProfileDto>.FailureResult($"Couldn`t find a user with id {userId}", ErrorType.NotFound);
        }

        try
        {
            user.ProfilePicUrl = filePath;
            await _db.SaveChangesAsync(ct);
            var dto = _mapper.Map<UserProfileDto>(user);

            return Result<UserProfileDto>.SuccessResult(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            
            return Result<UserProfileDto>.FailureResult(
                $"An error occured while updating user profile pic.", ErrorType.ServerError);
        }
    }
}