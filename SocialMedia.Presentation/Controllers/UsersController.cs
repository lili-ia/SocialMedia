using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Authorize]
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

        if (userId is null)
        {
            return Unauthorized("User is not authorized or token is invalid.");
        }

        var result = await _userService.GetOwnProfileInfoAsync(userId.Value, ct);

        return result.ToActionResult();
    }
    
    [HttpPut("me")]
    public async Task<IActionResult> UpdateOwnProfileAsync([FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var userId = _userContext.UserId;

        if (userId is null)
        {
            return Unauthorized("User is not authorized or token is invalid.");
        }
        
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

        if (userId is null)
        {
            return Unauthorized("User is not authorized or token is invalid.");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
            
        var result = await _userService.UpdateProfilePicAsync(userId.Value, stream, fileName, ct);
        
        return result.ToActionResult();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteOwnAccountAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPublicProfileAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsersAsync([FromQuery] string query)
    {
        throw new NotImplementedException();
    }
}