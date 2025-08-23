using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Authentication.ConfirmEmail;
using SocialMedia.Application.Authentication.Login;
using SocialMedia.Application.Authentication.Refresh;
using SocialMedia.Application.Authentication.Register;
using SocialMedia.Application.Authentication.RequestEmailConfirmation;
using SocialMedia.Application.Authentication.RequestPasswordReset;
using SocialMedia.Application.Authentication.ResetPassword;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.DTOs.Auth;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserContext _userContext;
    
    public AuthController(ISender sender, IUserContext userContext)
    {
        _sender = sender;
        _userContext = userContext;
    }
    
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            Username: registerRequest.Username,
            Email: registerRequest.Email,
            BirthDate: registerRequest.BirthDate,
            RawPassword: registerRequest.RawPassword);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var ipAddress = _userContext.IpAddress;
        var deviceInfo = _userContext.UserAgent;

        var command = new LoginUserCommand(
            Email: loginRequest.Email,
            Password: loginRequest.Password,
            IpAddress: ipAddress!,
            DeviceInfo: deviceInfo!);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        var ipAddress = _userContext.IpAddress;
        var deviceInfo = _userContext.UserAgent;

        var command = new RefreshTokenCommand(
            RefreshToken: refreshTokenDto.Token,
            IpAddress: ipAddress!,
            DeviceInfo: deviceInfo!);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("request-password-reset")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest passwordResetRequest)
    {
        var command = new RequestPasswordResetCommand(Email: passwordResetRequest.Email);

        var result = await _sender.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(
            Email: request.Email, 
            Token: request.Token, 
            request.NewPassword);
        
        var result = await _sender.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("request-email-confirmation")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestEmailConfirmation([FromBody] EmailConfirmationRequest request)
    {
        var command = new RequestEmailConfirmationCommand(Email: request.Email);
        
        var result = await _sender.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var command = new ConfirmEmailCommand(
            Email: request.Email, 
            Token: request.Token);
        
        var result = await _sender.Send(command);

        return result.ToActionResult();
    }
}