
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken ct)
    {
        _logger.LogInformation("Register method in controller executed");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.RegisterAsync(registerDto, ct);

        if (!result.Success) 
            return BadRequest(result.ErrorMessage);
        
        _logger.LogInformation($"User with email {registerDto.Email} registered successfully.");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        _logger.LogInformation("Login method in controller executed");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.LoginAsync(loginDto, ct);
        
        if (result.Success)
        {
            _logger.LogInformation($"User with email {loginDto.Email} logged in successfully.");
            
            return Ok(new {Token = result.Value});
        }
        
        _logger.LogWarning($"Failed login attempt for user with email {loginDto.Email}: {result.ErrorMessage}");
        
        return Unauthorized(result.ErrorMessage);
    }
}