using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Feed.GetFromFollowees;
using SocialMedia.Application.Feed.GetFromPopular;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/feed")]
public class FeedController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ISender _sender;

    public FeedController(IUserContext userContext, ISender sender)
    {
        _userContext = userContext;
        _sender = sender;
    }
    
     /// <summary>
    /// Retrieves the feed for the authenticated user based on their followees.
    /// </summary>
    /// <param name="page">Page number for pagination (default: 1).</param>
    /// <param name="pageSize">Number of posts per page (default: 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of posts from the user's followees.</returns>
    [HttpGet("followees")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeedFromFollowees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var command = new GetFeedFromFolloweesCommand(
            _userContext.UserId, 
            page, 
            pageSize);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves the popular feed for the authenticated user.
    /// </summary>
    /// <param name="page">Page number for pagination (default: 1).</param>
    /// <param name="pageSize">Number of posts per page (default: 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of popular posts.</returns>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(IReadOnlyList<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFeedFromPopular(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var command = new GetFeedFromPopularCommand(
            _userContext.UserId, 
            page, 
            pageSize);

        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}