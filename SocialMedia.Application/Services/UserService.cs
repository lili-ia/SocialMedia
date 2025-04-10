using System.Diagnostics;
using Infrastructure;
using Infrastructure.Contracts;
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
    
    public UserService(
        SocialMediaContext db, 
        IPasswordService passwordService,
        ILogger<UserService> logger, 
        IJwtService jwtService)
    {
        _db = db;
        _passwordService = passwordService;
        _logger = logger;
        _jwtService = jwtService;
    }
    
    public async Task<Result<User>> RegisterAsync(RegisterDto dto)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        
        if (userExists)
        {
            return Result<User>.FailureResult($"User with email {dto.Email} already exists");
        }

        var newUser = new User
        {
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.RawPassword),
            Username = dto.Username
        };

        try
        {
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result<User>.FailureResult($"An error occured while registering the user");
        }
    
        return Result<User>.SuccessResult(newUser);
    }

    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Result<string>.FailureResult("Invalid login attempt.");
        }

        var isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, dto.RawPassword);

        if (!isPasswordValid)
        {
            return Result<string>.FailureResult("Invalid login attempt.");
        }

        var token = _jwtService.GenerateToken(user.UserId.ToString(), user.Email);

        return Result<string>.SuccessResult(token);
    }
}