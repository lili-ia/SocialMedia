using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Messages.Delete;
using SocialMedia.Application.Messages.GetForChat;
using SocialMedia.Application.Messages.Send;
using SocialMedia.Application.Posts;
using SocialMedia.DTOs.Chat;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Authorize]
[Route("api/chats/{chatId}/messages")]
public class MessagesController(ISender sender, IUserContext userContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMessages(
        Guid chatId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 30, 
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMessagesForChatCommand
        {
            RequesterId = userContext.UserId,
            ChatId = chatId,
            Page = page,
            PageSize = pageSize
        }, ct);

        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(
        Guid chatId, 
        [FromForm] SendMessageRequest request, 
        CancellationToken ct = default)
    {
        var attachments = request.Files?
            .Select(f => new FileData(f.FileName, f.OpenReadStream()))
            .ToList();
        
        var result = await sender.Send(new SendMessageCommand
        (
            userContext.UserId,
            chatId,
            request.Content,
            request.ParentMessageId,
            attachments
        ), ct);

        return result.ToActionResult();
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteMessageCommand(userContext.UserId, messageId), ct);
        return result.ToActionResult();
    }
}