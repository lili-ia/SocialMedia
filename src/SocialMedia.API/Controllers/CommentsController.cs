using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Comments.Create;
using SocialMedia.Application.Comments.Delete;
using SocialMedia.Application.Comments.GetAllForPost;
using SocialMedia.Application.Comments.GetById;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.DTOs.Comment;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Authorize(Roles = "User")]
[Produces("application/json")]
[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ISender _sender;

    public CommentsController(IUserContext userContext, ISender sender)
    {
        _userContext = userContext;
        _sender = sender;
    }

    /// <summary>
    /// Gets a comment by its ID.
    /// </summary>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The comment details.</returns>
    [HttpGet("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommentById([FromRoute] Guid commentId, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new GetCommentByIdCommand(
            CommentId: commentId, 
            TargetUserId: userId);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Creates a new comment for a specific post.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="request">The comment creation request containing text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment.</returns>
    [HttpPost("posts/{postId:guid}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateComment(
        [FromRoute] Guid postId, 
        [FromBody] CreateCommentRequest request, 
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new CreateCommentCommand(
            Text: request.Text, 
            PostId: postId, 
            UserId: userId);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Deletes a comment by its ID.
    /// </summary>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success status if deleted.</returns>
    [HttpDelete("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new DeleteCommentCommand(
            CommentId: commentId, 
            UserId: userId);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Gets all comments for a specific post with pagination.
    /// </summary>
    /// <param name="postId">The post ID.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Number of comments per page (default: 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of comments.</returns>
    [HttpGet("posts/{postId:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommentsForPost(
        [FromRoute] Guid postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new GetAllCommentsForPostCommand(
            PostId: postId, 
            TargetUserId : userId, 
            Page: page,
            PageSize: pageSize);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}