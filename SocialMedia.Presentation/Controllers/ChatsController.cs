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
    public async Task<IActionResult> GetAllChats(CancellationToken cancellationToken)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _chatService.GetAllChats(userIntId, cancellationToken);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetMessagesForThisChat(
        CancellationToken cancellationToken,
        [FromRoute] int chatId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var skipCount = (page - 1) * pageSize;
        var result = await _chatService.GetMessagesByChatId(cancellationToken, chatId, skipCount, pageSize);

        return result.ToActionResult();
    }
}