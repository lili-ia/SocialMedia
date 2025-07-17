using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Comment;
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
    public async Task<IActionResult> GetCommentAsync(Guid commentId, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetCommentAsync(commentId, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentRequest request, Guid postId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out var userGuidId);
        var result = await _commentService.CreateCommentAsync(request.Text, postId, userGuidId, cancellationToken);

        return result.ToActionResult();
    }
    
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateCommentAsync([FromBody] CreateCommentRequest request, [FromRoute] Guid commentId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userIntId);
        var result = await _commentService.UpdateCommentAsync(commentId, request.Text, userIntId, cancellationToken);

        return result.ToActionResult();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _commentService.DeleteCommentAsync(commentId, userGuidId, cancellationToken);
        
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCommentsForPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetCommentsForPostAsync(postId, cancellationToken);
        
        return result.ToActionResult();
    }
}