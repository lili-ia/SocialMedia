using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Posts;
using SocialMedia.Application.Users.Deactivate;
using SocialMedia.Application.Users.GetPrivateInfo;
using SocialMedia.Application.Users.GetPublicInfo;
using SocialMedia.Application.Users.Search;
using SocialMedia.Application.Users.Update;
using SocialMedia.DTOs.User;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Produces("application/json")]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ISender _sender;

    public UsersController(IUserContext userContext, ISender sender)
    {
        _userContext = userContext;
        _sender = sender;
    }

    /// <summary>
    /// Retrieves the authenticated user's private profile information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Private user information.</returns>
    [Authorize(Roles = "User")]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserPrivateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOwnProfile(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new GetPrivateUserInfoCommand(userId);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Updates the authenticated user's profile.
    /// </summary>
    /// <param name="request">Profile update request containing bio, birth date, or profile picture.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated user information.</returns>
    [Authorize(Roles = "User")]
    [HttpPut("me")]
    [ProducesResponseType(typeof(UpdateUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOwnProfile(
        [FromBody] UpdateUserRequest request, 
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        FileData? file = null;

        if (request.ProfilePic is not null)
        {
            await using var ms = new MemoryStream();
            await request.ProfilePic.CopyToAsync(ms, cancellationToken);
            file = new FileData(FileName: request.ProfilePic.FileName, Content: request.ProfilePic.OpenReadStream());
        }
        
        var command = new UpdateUserCommand(
            UserId: userId, 
            BirthDate: request.BirthDate, 
            ProfilePic: file, 
            Bio: request.Bio);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Deactivates the authenticated user's profile (soft delete).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of the operation.</returns>
    [Authorize(Roles = "User")]
    [HttpDelete("me")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateOwnProfile(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new DeactivateUserCommand(userId);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves a public profile of a user by their ID.
    /// </summary>
    /// <param name="userId">Target user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Public user information.</returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserPublicDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicProfile([FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        var forUserId = _userContext.UserIdOrNull;

        var command = new GetPublicUserInfoCommand(
            UserId: userId, 
            ForUserId: forUserId);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Searches for users by username with optional pagination.
    /// </summary>
    /// <param name="username">Username to search for (partial match).</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching users.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<UserPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string username, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var forUserId = _userContext.UserIdOrNull;

        var command = new SearchUsersCommand(
            ForUserId: forUserId, 
            Username: username, 
            Page: page, 
            PageSize: pageSize);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}