using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Auth;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserContext _userContext;
    
    public AuthController(ILogger<AuthController> logger, IAuthService authService, IUserContext userContext)
    {
        _logger = logger;
        _authService = authService;
        _userContext = userContext;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(registerDto, ct);

        if (result.Success)
        {
            _logger.LogInformation("User with email {Email} registered successfully.", registerDto.Email);
        }
        else
        {
            _logger.LogWarning("Failed register attempt for email {Email}: {ResultErrorMessage}.", registerDto.Email, result.ErrorMessage);
        }
        
        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var ip = _userContext.IpAddress;
        var userAgent = _userContext.UserAgent;
        
        var result = await _authService.LoginAsync(loginDto, ip, userAgent, ct);

        if (result.Success)
        {
            _logger.LogInformation("User with email {Email} logged in successfully.", loginDto.Email);
        }
        else
        {
            _logger.LogWarning("Failed login attempt for user with email {Email}: {ResultErrorMessage}.", loginDto.Email, result.ErrorMessage);
        }

        return result.ToActionResult();
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var ip = _userContext.IpAddress;
        var userAgent = _userContext.UserAgent;
        
        var result = await _authService.RefreshTokenAsync(dto.Token, ip, userAgent, ct);

        if (result.Success)
        {
            _logger.LogInformation("Access token successfully refreshed.");
        }
        else
        {
            _logger.LogWarning("Failed token refresh attempt: {ResultErrorMessage}.", result.ErrorMessage);
        }

        return result.ToActionResult();
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordResetAsync([FromBody] RequestPasswordResetRequest request)
    {
        var result = await _authService.RequestPasswordResetAsync(request.Email);

        if (result.Success)
        {
            _logger.LogInformation("Password reset successfully requested.");
        }
        else
        {
            _logger.LogWarning("Failed password reset request: {ResultErrorMessage}.", result.ErrorMessage);
        }
        
        return result.ToActionResult();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        
        if (result.Success)
        {
            _logger.LogInformation("Password successfully reset.");
        }
        else
        {
            _logger.LogWarning("Failed password reset: {ResultErrorMessage}.", result.ErrorMessage);
        }

        return result.ToActionResult();
    }

    [HttpPost("send-email-confirmation")]
    public async Task<IActionResult> SendEmailConfirmationAsync([FromBody] EmailConfirmationRequest request)
    { 
        var result = await _authService.SendEmailConfirmationAsync(request.Email);
        
        if (result.Success)
        {
            _logger.LogInformation("Email confirmation successfully sent.");
        }
        else
        {
            _logger.LogWarning("Failed email confirmation request: {ResultErrorMessage}.", result.ErrorMessage);
        }
        
        return result.ToActionResult();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request.Email, request.Token);
        
        if (result.Success)
        {
            _logger.LogInformation("Email successfully confirmed.");
        }
        else
        {
            _logger.LogWarning("Failed email confirmation: {ResultErrorMessage}.", result.ErrorMessage);
        }
        
        return result.ToActionResult();
    }
}