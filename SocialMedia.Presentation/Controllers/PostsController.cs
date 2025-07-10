using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Route("api/posts")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    
    public PostsController(IPostService postService)
    {
        _postService = postService;
    }
    
    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostAsync([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var result = await _postService.GetPostByIdAsync(postId, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetPostsOfUsernameAsync([FromQuery] string username, CancellationToken cancellationToken)
    {
        var result = await _postService.GetPostsOfUsernameAsync(username, cancellationToken);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePostAsync([FromBody] UpdatePostDto dto, [FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _postService.UpdatePostAsync(dto, postId, userGuidId, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto dto, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");

        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _postService.CreatePostAsync(dto, userGuidId, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePostAsync([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _postService.DeletePostAsync(postId, userGuidId, cancellationToken);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("hidden")]
    public async Task<IActionResult> GetHiddenPostsAsync(CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _postService.GetPostsByUserAndActiveStatusAsync(userGuidId, isActive: false, cancellationToken);
        
        return result.ToActionResult();
    }
}