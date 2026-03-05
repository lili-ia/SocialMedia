using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Chats.AddParticipant;
using SocialMedia.Application.Chats.Create;
using SocialMedia.Application.Chats.GetMy;
using SocialMedia.Application.Chats.Leave;
using SocialMedia.Application.Contracts;
using SocialMedia.DTOs.Chat;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Authorize]
[Route("api/chats")]
public class ChatsController(ISender sender, IUserContext userContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateChatCommand
        {
            RequesterId = userContext.UserId,
            IsGroup = request.IsGroup,
            GroupName = request.GroupName,
            ParticipantIds = request.ParticipantIds
        }, ct);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyChats(CancellationToken ct)
    {
        var result = await sender.Send(new GetMyChatsCommand(userContext.UserId), ct);
        return result.ToActionResult();
    }

    [HttpPost("{chatId}/participants")]
    public async Task<IActionResult> AddParticipant(
        Guid chatId, 
        [FromBody] AddParticipantRequest request, 
        CancellationToken ct)
    {
        var result = await sender.Send(new AddParticipantCommand(userContext.UserId, chatId, request.UserId), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{chatId}/leave")]
    public async Task<IActionResult> LeaveChat(Guid chatId, CancellationToken ct)
    {
        var result = await sender.Send(new LeaveChatCommand(userContext.UserId, chatId), ct);
        return result.ToActionResult();
    }
}