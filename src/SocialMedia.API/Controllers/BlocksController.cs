using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Blocks.Create;
using SocialMedia.Application.Blocks.Delete;
using SocialMedia.Application.Blocks.GetBlockedUsers;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Block;
using SocialMedia.DTOs.Block;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Authorize(Roles = "User")]
[Produces("application/json")]
[ApiController]
[Route("api/blocks")]
public class BlocksController(IUserContext userContext, ISender sender) : ControllerBase
{
    /// <summary>
    /// Blocks user.
    /// </summary>
    /// <param name="request">Block request containing the ID of the user to block.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about block status.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BlockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BlockUser([FromBody] CreateBlockRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBlockCommand(userContext.UserId, request.BlockedId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Unblocks user.
    /// </summary>
    /// <param name="blockedUserId">ID of the user to unblock.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Information about block status.</returns>
    [HttpDelete("{blockedUserId:guid}")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockUser(Guid blockedUserId, CancellationToken cancellationToken)
    {
        var command = new DeleteBlockCommand(userContext.UserId, blockedUserId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves the list of users blocked by a given user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user previews representing blocked users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BlockedUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlockedUsers(CancellationToken cancellationToken)
    {
        var command = new GetBlockedUsersCommand(userContext.UserId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}