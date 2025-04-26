using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;
using SocialMedia.Extensions;

namespace SocialMedia.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public UsersController(IUserService userService, IWebHostEnvironment webHostEnvironment)
    {
        _userService = userService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto, [FromRoute] int userId, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateProfileAsync(dto, userId, cancellationToken);

        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAuthUserInfo(CancellationToken ct)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        int.TryParse(userStringId, out int userIntId);
        var result = await _userService.GetUserInfoAsync(userIntId, ct);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("{userId}/pic")]
    public async Task<IActionResult> UploadProfilePic([FromRoute] int userId, IFormFile file, CancellationToken ct)
    {
        var userStringId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userStringId == null)
            return Unauthorized("User not found");
        
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        var profilePicUrl = $"/uploads/{fileName}";
        int.TryParse(userStringId, out int userIntId);
        
        var result = await _userService.UpdateProfilePic(userIntId, profilePicUrl, ct);

        return result.ToActionResult();
    }
}