using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;
    
    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllChatsAsync(CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        Guid.TryParse(userStringId, out Guid userGuidId);
        var result = await _chatService.GetAllChatsAsync(userGuidId, cancellationToken);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetMessagesForThisChatAsync(
        CancellationToken cancellationToken,
        [FromRoute] Guid chatId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var skipCount = (page - 1) * pageSize;
        var result = await _chatService.GetMessagesByChatIdAsync(cancellationToken, chatId, skipCount, pageSize);

        return result.ToActionResult();
    }
}