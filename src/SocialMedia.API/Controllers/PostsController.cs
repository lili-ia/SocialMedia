using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Posts;
using SocialMedia.Application.Posts.ChangeActiveStatus;
using SocialMedia.Application.Posts.Create;
using SocialMedia.Application.Posts.Delete;
using SocialMedia.Application.Posts.GetById;
using SocialMedia.Application.Posts.GetMyInactive;
using SocialMedia.Application.Posts.GetPublicOfUsername;
using SocialMedia.Application.Posts.Update;
using SocialMedia.DTOs.Post;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ISender _sender;
    public PostsController(IUserContext userContext, ISender sender)
    {
        _userContext = userContext;
        _sender = sender;
    }
    
    [AllowAnonymous]
    [HttpGet("by-id/{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserIdOrNull;
        
        var command = new GetPostByIdCommand(
            PostId: id,
            TargetUserId: userId);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("by-username/{username}")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublicPostsOfUsername(
        [FromRoute] string username, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserIdOrNull;

        var command = new GetPublicPostsOfUsernameCommand(
            AuthorUsername: username,
            TargetUserId: userId,
            Page: page, 
            PageSize: pageSize);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize(Roles = "User")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] Guid id, 
        [FromBody] UpdatePostRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var command = new UpdatePostCommand(
            PostId: id, 
            UserId: userId, 
            Text: request.Text);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize(Roles = "User")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        List<FileData>? files = null;

        if (request.Files is not null)
        {
            files = [];

            foreach (var f in request.Files)
            {
                await using var ms = new MemoryStream();
                await f.CopyToAsync(ms, cancellationToken);
                files.Add(new FileData(FileName: f.FileName, Content: f.OpenReadStream()));
            }
        }

        var command = new CreatePostCommand(
            UserId: userId, 
            Text: request.Text, 
            Files: files);
        
        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(nameof(GetPostById), new { Id = result.Value }, result.Value);
    }

    [Authorize(Roles = "User")]
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePostActiveStatus(
        [FromRoute] Guid id, 
        [FromBody] ChangePostActiveStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new ChangePostActiveStatusCommand(
            PostId: id, 
            UserId: userId, 
            ActiveStatus: request.ActiveStatus);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize(Roles = "User")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePost([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var command = new DeletePostCommand(
            PostId: id, 
            UserId: userId);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize(Roles = "User")]
    [HttpGet("me/inactive")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyInactive(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;

        var command = new GetMyInactivePostsCommand(
            UserId: userId, 
            Page: page, 
            PageSize: pageSize);
        
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}