using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Authentication.ConfirmEmail;
using SocialMedia.Application.Authentication.Login;
using SocialMedia.Application.Authentication.Refresh;
using SocialMedia.Application.Authentication.Register;
using SocialMedia.Application.Authentication.RequestEmailConfirmation;
using SocialMedia.Application.Authentication.RequestPasswordReset;
using SocialMedia.Application.Authentication.ResetPassword;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Auth;
using SocialMedia.DTOs.Auth;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Produces("application/json")]
[Route("api/auth")]
[ApiController]
[Tags("Authentication")]
public class AuthController(ISender sender, IUserContext userContext) : ControllerBase
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// - Password must be at least 8 characters.
    /// - Email must be unique.
    /// - User will be created in 'Pending' status until email is confirmed.
    /// </remarks>
    /// <response code="200">User registered successfully. Check email for confirmation link.</response>
    /// <response code="400">Validation failed (e.g., invalid email format).</response>
    /// <response code="409">Email or Username is already taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
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

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Authenticates a user and returns Access/Refresh tokens.
    /// </summary>
    /// <response code="200">Returns the JWT Access Token and a Refresh Token.</response>
    /// <response code="401">Invalid credentials or account doesn't exist.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var ipAddress = userContext.IpAddress;
        var deviceInfo = userContext.UserAgent;

        var command = new LoginUserCommand(
            Email: loginRequest.Email,
            Password: loginRequest.Password,
            IpAddress: ipAddress!,
            DeviceInfo: deviceInfo!);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Generates a new Access Token using a valid Refresh Token.
    /// </summary>
    /// <response code="200">New token pair generated.</response>
    /// <response code="401">Refresh token is expired, revoked, or invalid.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto, CancellationToken cancellationToken)
    {
        var ipAddress = userContext.IpAddress;
        var deviceInfo = userContext.UserAgent;

        var command = new RefreshTokenCommand(
            RefreshToken: refreshTokenDto.Token,
            IpAddress: ipAddress!,
            DeviceInfo: deviceInfo!);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Initiates a password reset flow.
    /// </summary>
    /// <remarks>
    /// For security reasons, this endpoint returns 200 OK even if the email doesn't exist 
    /// to prevent account enumeration.
    /// </remarks>
    /// <response code="200">If the account exists, a reset email has been sent.</response>
    /// <response code="429">Rate limit exceeded. Please wait before requesting again.</response>
    [HttpPost("request-password-reset")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest passwordResetRequest)
    {
        var command = new RequestPasswordResetCommand(Email: passwordResetRequest.Email);

        var result = await sender.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Resets the password using a valid secret token.
    /// </summary>
    /// <response code="200">Password changed successfully.</response>
    /// <response code="401">The token is invalid, expired, or already used.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(
            Email: request.Email, 
            Token: request.Token, 
            request.NewPassword);
        
        var result = await sender.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Re-sends the email confirmation link.
    /// </summary>
    /// <response code="200">Confirmation email sent.</response>
    /// <response code="429">Too many attempts. Please try again later.</response>
    [HttpPost("request-email-confirmation")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RequestEmailConfirmation([FromBody] EmailConfirmationRequest request)
    {
        var command = new RequestEmailConfirmationCommand(Email: request.Email);
        
        var result = await sender.Send(command);

        return result.ToActionResult();
    }

    /// <summary>
    /// Finalizes email confirmation via a link token.
    /// </summary>
    /// <param name="token">The secret token from the email link.</param>
    /// <param name="email">The user's email address.</param>
    /// <response code="200">Email verified successfully. Account is now active.</response>
    /// <response code="401">Token is invalid or expired.</response>
    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var command = new ConfirmEmailCommand(
            Email: email, 
            Token: token);
        
        var result = await sender.Send(command);

        return result.ToActionResult();
    }
}