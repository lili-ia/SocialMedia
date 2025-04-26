using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class AuthService : IAuthService
{
    private readonly SocialMediaContext _db;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;
    
    public AuthService(SocialMediaContext db, IMapper mapper, IPasswordService passwordService, ILogger<AuthService> logger, IJwtService jwtService)
    {
        _db = db;
        _mapper = mapper;
        _passwordService = passwordService;
        _logger = logger;
        _jwtService = jwtService;
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

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, dto.RawPassword);

        if (!isPasswordValid)
        {
            return Result<AuthResponseDto>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var accessToken = _jwtService.GenerateToken(user.UserId.ToString(), user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();
        // TODO: include IP and Device Info 
        refreshToken.UserId = user.UserId;

        try
        {
            await _db.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            var authResponse = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

            return Result<AuthResponseDto>.SuccessResult(authResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result<AuthResponseDto>.FailureResult(
                $"Internal server error. Try later.", ErrorType.ServerError);
        }
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken: cancellationToken);

        if (token == null || token.Expires < DateTime.UtcNow)
        {
            return Result<AuthResponseDto>.FailureResult(
                "Invalid refresh token", ErrorType.Unauthorized);
        }

        var user = await _db.Users.FindAsync(token.UserId);
        
        if (user == null)
        {
            return Result<AuthResponseDto>.FailureResult(
                $"Couldnt find a user.", ErrorType.NotFound);
        }

        var newAccessToken = _jwtService.GenerateToken(token.UserId.ToString(), user.Email);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        token.IsRevoked = true;

        try
        {
            _db.RefreshTokens.Update(token);
            await _db.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Result<AuthResponseDto>.FailureResult(
                $"Internal server error. Try later.", ErrorType.ServerError);
        }

        var authResponse = new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };

        return Result<AuthResponseDto>.SuccessResult(authResponse);
    }
}