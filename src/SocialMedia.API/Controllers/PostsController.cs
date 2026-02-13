using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Posts;
using SocialMedia.Application.Posts.ChangeHiddenStatus;
using SocialMedia.Application.Posts.Create;
using SocialMedia.Application.Posts.Delete;
using SocialMedia.Application.Posts.GetById;
using SocialMedia.Application.Posts.GetMyHidden;
using SocialMedia.Application.Posts.GetPublicOfUser;
using SocialMedia.Application.Posts.Update;
using SocialMedia.DTOs.Post;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Authorize(Policy = "ActiveUser", Roles = "User")]
[Produces("application/json")]
[ApiController]
[Route("api/posts")]
public class PostsController(IUserContext userContext, ISender sender) : ControllerBase
{
    /// <summary>
    /// Retrieves a post by its unique identifier.
    /// </summary>
    /// <param name="id">Post identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Anonymous users can access public posts.
    /// Hidden posts are accessible only to their owner.
    /// </remarks>
    /// <returns>
    /// <response code="200">Post found.</response>
    /// <response code="404">Post not found or not accessible by this user.</response>
    /// </returns>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserIdOrNull;
        
        var command = new GetPostByIdCommand(
            PostId: id,
            TargetUserId: userId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Retrieves public (non-hidden) posts of a specific user.
    /// </summary>
    /// <param name="userId">Author identifier (optional).</param>
    /// <param name="username">Author username (optional if userId provided).</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Either <paramref name="userId"/> or <paramref name="username"/> must be provided.
    /// 
    /// If there is a block relationship between the requesting user
    /// and the author, the API returns 404.
    /// 
    /// Only non-hidden posts are returned.
    /// </remarks>
    /// <returns>
    /// <response code="200">List of public posts.</response> 
    /// <response code="400">400 - Validation failed.</response> 
    /// <response code="404">404 - Author not found or blocked.</response> 
    /// </returns>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicPostsOfUser(
        [FromQuery] Guid? userId,
        [FromQuery] string? username, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var targetUserId = userContext.UserIdOrNull;

        var command = new GetPublicPostsOfUserCommand(
            AuthorUserId: userId,
            AuthorUsername: username,
            TargetUserId: targetUserId,
            Page: page, 
            PageSize: pageSize);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">Post identifier.</param>
    /// <param name="request">Updated post data including text and files.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Only the post owner can update the post.
    /// 
    /// Files provided in request are uploaded and attached to the post.
    /// Existing files can be preserved using <c>KeptStorageKeys</c>.
    /// </remarks>
    /// <returns>
    /// <response code="200">Post successfully updated.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">User does not own the post.</response>
    /// <response code="404">Post not found.</response>
    /// <response code="500">File upload or internal error.</response>
    /// </returns>
    [Consumes("multipart/form-data")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] Guid id, 
        [FromForm] UpdatePostRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        List<FileData>? files = null;

        if (request.NewFiles is not null)
        {
            files = [];

            foreach (var f in request.NewFiles)
            {
                var ms = new MemoryStream();
                await f.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                files.Add(new FileData(f.FileName, ms));
            }
        }
        
        var command = new UpdatePostCommand(
            PostId: id, 
            UserId: userId, 
            Text: request.Text,
            NewFiles: files,
            KeptStorageKeys: request.KeptStorageKeys);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="request">Post content and optional files.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// The post is created as active (not hidden).
    /// 
    /// Uploaded files are stored and linked to the post.
    /// For each uploaded image, metadata (width/height) is extracted.
    /// 
    /// If file upload fails, the operation returns an internal error.
    /// </remarks>
    /// <returns>
    /// <response code="200">Post successfully updated.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">User does not own the post.</response>
    /// <response code="500">File upload or internal error.</response>
    /// </returns>
    [Consumes("multipart/form-data")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        List<FileData>? files = null;

        if (request.Files is not null)
        {
            files = [];

            foreach (var f in request.Files)
            {
                var ms = new MemoryStream();
                await f.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                files.Add(new FileData(f.FileName, ms));
            }
        }

        var command = new CreatePostCommand(
            UserId: userId, 
            Text: request.Text, 
            Files: files);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Changes post hidden status.
    /// </summary>
    /// <param name="id">Post identifier.</param>
    /// <param name="request">Visibility change request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Only the post owner can change visibility.
    /// 
    /// Returns conflict if the post already has the requested status.
    /// </remarks>
    /// <returns>
    /// <response code="200">Status changed successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">User does not own the post.</response>
    /// <response code="404">Post not found.</response>
    /// </returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePostActiveStatus(
        [FromRoute] Guid id, 
        [FromBody] ChangePostActiveStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;

        var command = new ChangePostHiddenStatusCommand(
            PostId: id, 
            UserId: userId, 
            MustBeHidden: request.MustBeHidden);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="id">Post identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Only the post owner can delete the post.
    /// </remarks>
    /// <returns>
    /// <response code="200">Successfully deleted.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">User does not own the post.</response>
    /// <response code="404">Post not found.</response>
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;

        var command = new DeletePostCommand(
            PostId: id, 
            UserId: userId);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    /// <summary>
    /// Retrieves hidden posts of the current authenticated user.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// Only hidden posts belonging to the authenticated user are returned.
    /// 
    /// Each post includes presigned URLs for associated files.
    /// </remarks>
    /// <returns>
    /// <response code="200">List of hidden posts.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// </returns>
    [HttpGet("me/hidden")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyHidden(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;

        var command = new GetMyHiddenPostsCommand(
            UserId: userId, 
            Page: page, 
            PageSize: pageSize);
        
        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}