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
    public async Task<IActionResult> GetPost([FromRoute] int postId)
    {
        var result = await _postService.GetPost(postId);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetPostsOfUsername([FromQuery] string username)
    {
        var result = await _postService.GetPostsOfUsername(username);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPut("{postId}")]
    public async Task<IActionResult> UpdatePost([FromRoute] UpdatePostDto dto)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _postService.UpdatePost(dto, userIntId);
        
        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");

        int.TryParse(userStringId, out int userIntId);
        var result = await _postService.CreatePost(dto, userIntId);

        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePost([FromBody] int postId)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _postService.DeletePost(postId, userIntId);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("hidden")]
    public async Task<IActionResult> GetHiddenPosts()
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _postService.GetPostsByUserAndActiveStatus(userIntId, isActive: false);
        
        return result.ToActionResult();
    }
}