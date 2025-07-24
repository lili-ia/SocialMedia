using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Route("api/posts")]
public class LikesController : ControllerBase
{
    private readonly ILikeService _likeService;
    
    public LikesController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [Authorize]
    [HttpPost("{postId}/like")]
    public async Task<IActionResult> LikePostAsync([FromRoute] Guid postId, CancellationToken ct)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);

        var result = await _likeService.LikePostAsync(postId, userGuidId, ct);

        return result.ToActionResult();
    }
}