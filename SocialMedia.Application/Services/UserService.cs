using AutoMapper;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.DTOs;

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
    
    public async Task<Result<User>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Email == dto.Email, cancellationToken);
        
        if (userExists)
        {
            return Result<User>.FailureResult(
                $"User with email {dto.Email} already exists", ErrorType.Validation);
        }

        var newUser = _mapper.Map<User>(dto);
        newUser.PasswordHash = _passwordService.HashPassword(dto.RawPassword);

        try
        {
            await _db.Users.AddAsync(newUser, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result<User>.FailureResult(
                $"An error occured while registering the user", ErrorType.ServerError);
        }
    
        return Result<User>.SuccessResult(newUser);
    }

    public async Task<Result<string>> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (user == null)
        {
            return Result<string>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, dto.RawPassword);

        if (!isPasswordValid)
        {
            return Result<string>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email);

        return Result<string>.SuccessResult(token);
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
}