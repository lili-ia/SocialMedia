using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Like;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Likes.Create;
using SocialMedia.Application.Likes.DeleteLike;
using SocialMedia.Application.Likes.GetPostLikers;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Authorize(Roles = "User")]
[Produces("application/json")]
[ApiController]
[Route("api/posts/{postId:guid}/likes")]
public class LikesController(IUserContext userContext, ISender sender) : ControllerBase
{
    /// <summary>
    /// Likes a post for the current user.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [ProducesResponseType(typeof(PostLikeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> LikePostAsync([FromRoute] Guid postId, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;

        var command = new CreatePostLikeCommand(
            LikerId: userId, 
            PostId: postId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Removes the current user's like from the post.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="cancellationToken"></param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid postId, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        
        var command = new DeletePostLikeCommand(
            PostId: postId, 
            LikerId: userId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Gets a paginated list of users who liked the post.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <param name="cancellationToken"></param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserPreviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostLikers(
        [FromRoute] Guid postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var command = new GetPostLikersCommand(
            PostId: postId, 
            TargetUserId: userContext.UserId,
            Page: page,
            PageSize: pageSize);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}