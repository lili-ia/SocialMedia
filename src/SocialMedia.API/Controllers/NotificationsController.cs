using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Notifications.Get;
using SocialMedia.Application.Notifications.MarkAsRead;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController(ISender sender, IUserContext userContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetNotificationsCommand(userContext.UserId, page, pageSize), ct);

        return result.ToActionResult();
    }

    [HttpPut("read")]
    public async Task<IActionResult> MarkAsRead(CancellationToken ct)
    {
        var result = await sender.Send(new MarkNotificationsAsReadCommand(userContext.UserId), ct);
        return result.ToActionResult();
    }
}