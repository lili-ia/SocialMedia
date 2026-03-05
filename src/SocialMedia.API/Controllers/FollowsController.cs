using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Follow;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Follows.Create;
using SocialMedia.Application.Follows.Delete;
using SocialMedia.Application.Follows.GetFolloweesOfUser;
using SocialMedia.Application.Follows.GetFollowersOfUser;
using SocialMedia.DTOs.Follow;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/follows")]
public class FollowsController(ISender sender, IUserContext userContext) : ControllerBase
{
    /// <summary>
    /// Follows a user.
    /// </summary>
    /// <param name="request">Follow request containing the ID of the user to follow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about follow status and follower count.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FollowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FollowUser([FromBody] CreateFollowRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateFollowCommand(
            FollowerId: userContext.UserId, 
            FolloweeId: request.FolloweeId);
        
        var result = await sender.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }

    /// <summary>
    /// Unfollows a user.
    /// </summary>
    /// <param name="followeeId">ID of the user to unfollow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about follow status and updated follower count.</returns>
    [HttpDelete("{followeeId:guid}")]
    [ProducesResponseType(typeof(FollowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnfollowUser([FromRoute] Guid followeeId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteFollowCommand(
            FollowerId: userContext.UserId, 
            FolloweeId: followeeId);
        
        var result = await sender.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves the list of users followed by a given user.
    /// </summary>
    /// <param name="userId">ID of the user whose followees to retrieve.</param>
    /// <param name="page">Page number to retrieve.</param>
    /// <param name="pageSize">Followees count to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user previews representing followees.</returns>
    [HttpGet("{userId:guid}/followees")]
    [ProducesResponseType(typeof(IReadOnlyList<UserPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFollowees(
        [FromRoute] Guid userId, 
        [FromRoute] int page = 1,
        [FromRoute] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var command = new GetFolloweesOfUserCommand(
            UserId: userId, 
            ForUserId: userContext.UserIdOrNull,
            Page: page,
            PageSize: pageSize);
        
        var result = await sender.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Retrieves the list of users who follow a given user.
    /// </summary>
    /// <param name="userId">ID of the user whose followers to retrieve.</param>
    /// <param name="page">Page number to retrieve.</param>
    /// <param name="pageSize">Followees count to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user previews representing followers.</returns>
    [HttpGet("{userId:guid}/followers")]
    [ProducesResponseType(typeof(IReadOnlyList<UserPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFollowers(
        [FromRoute] Guid userId, 
        [FromRoute] int page = 1,
        [FromRoute] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var command = new GetFollowersOfUserCommand(
            UserId: userId, 
            ForUserId: userContext.UserIdOrNull, 
            Page: page,
            PageSize: pageSize);
        
        var result = await sender.Send(command, cancellationToken);
        
        return result.ToActionResult();
    }
}