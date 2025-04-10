using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;

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
    

    [HttpGet]
    public async Task<IActionResult> GetPost([FromRoute] int postId)
    {
        var result = await _postService.GetPost(postId);

        return result switch
        {
            
        };
    }

    [HttpGet]
    public IActionResult<IActionResult> GetPostsOfUsername([FromQuery] string username)
    {
        
    }

    public IActionResult<IActionResult> UpdatePost()
    {
        
    }
    
    public IActionResult<IActionResult> CreatePost()
    {
        
    }
    
    public IActionResult<IActionResult> DeletePost()
    {
        
    }
    // Get Posts by Active Status
    // Update Post
    // Create Post
    // Delete Post
}