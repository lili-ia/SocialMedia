using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.ActionFilters;
using SocialMedia.Application.Contracts;
using SocialMedia.Extensions;
using SocialMedia.Shared.DTOs.User;

namespace SocialMedia.Controllers;

[Authorize]
[ServiceFilter(typeof(RequireUserIdNotNullFilter))]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserContext _userContext;
    
    public UsersController(IUserService userService, IUserContext userContext)
    {
        _userService = userService;
        _userContext = userContext;
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> GetOwnProfileAsync(CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var result = await _userService.GetOwnProfileInfoAsync(userId.Value, ct);

        return result.ToActionResult();
    }
    
    [HttpPut("me")]
    public async Task<IActionResult> UpdateOwnProfileAsync([FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var userId = _userContext.UserId;
        var result = await _userService.UpdateProfileAsync(dto, userId.Value, ct);
        
        return result.ToActionResult();
    }
    
    [HttpPut("me/profile-pic")]
    public async Task<IActionResult> UpdateProfilePicAsync(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }
        
        var userId = _userContext.UserId;
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
            
        var result = await _userService.UpdateProfilePicAsync(userId.Value, stream, fileName, ct);
        
        return result.ToActionResult();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteOwnAccountAsync(CancellationToken ct)
    {
        var userId = _userContext.UserId;
        var result = await _userService.DeleteUserAsync(userId.Value, ct);

        return result.ToActionResult();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPublicProfileAsync([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _userService.GetPublicUserInfoAsync(userId, ct);

        return result.ToActionResult();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsersAsync(
        [FromQuery] string query, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query cannot be empty.");
        }

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var result = await _userService.SearchUsersAsync(query, pageNumber, pageSize, ct);

        return result.ToActionResult();
    }
}