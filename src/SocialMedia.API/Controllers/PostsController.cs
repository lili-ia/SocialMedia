using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ActionFilters;
using SocialMedia.Application.Contracts;
using SocialMedia.Extensions;
using SocialMedia.Shared.DTOs.Post;

namespace SocialMedia.Controllers;

[Authorize]
[ServiceFilter(typeof(RequireUserIdNotNullFilter))]
[Route("api/posts")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IUserContext _userContext;
    
    public PostsController(IPostService postService, IUserContext userContext)
    {
        _postService = postService;
        _userContext = userContext;
    }
    
    [AllowAnonymous]
    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostByIdAsync([FromRoute] Guid postId, CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var result = await _postService.GetPostByIdAsync(postId, userId.Value, ct);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetPublicPostsOfUsernameAsync(
        [FromRoute] string username, 
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default
        )
    {
        var result = await _postService.GetPostsOfUsernameAsync(username, page, pageSize, ct);

        return result.ToActionResult();
    }
    
    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePostAsync([FromBody] UpdatePostDto dto, [FromRoute] Guid postId, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _postService.UpdatePostAsync(dto, postId, userId.Value, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest request, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _postService.CreatePostAsync(request, userId.Value, ct);

        return result.ToActionResult();
    }
    
    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePostAsync([FromRoute] Guid postId, CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var result = await _postService.DeletePostAsync(postId, userId.Value, ct);

        return result.ToActionResult();
    }
    
    [HttpGet("me/hidden")]
    public async Task<IActionResult> GetHiddenPostsAsync(CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var result = await _postService.GetMyInactivePosts(userId.Value, ct);
        
        return result.ToActionResult();
    }
}