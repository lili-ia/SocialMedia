using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    
    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet("{commentId}")]
    public async Task<IActionResult> GetComment(int commentId, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetComment(commentId, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentDTO dto, int postId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _commentService.CreateComment(dto.Text, postId, userIntId, cancellationToken);

        return result.ToActionResult();
    }
    
    [HttpPut("{commentID}")]
    public async Task<IActionResult> UpdateComment([FromBody] CommentDTO dto, [FromRoute] int commentId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _commentService.UpdateComment(commentId, dto.Text, userIntId, cancellationToken);

        return result.ToActionResult();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int commentId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _commentService.DeleteComment(commentId, userIntId, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCommentsForPost(int postId, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetCommentsForPost(postId, cancellationToken);
        
        return result.ToActionResult();
    }
}