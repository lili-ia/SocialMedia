using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Security;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            
            return Guid.TryParse(userIdClaim?.Value, out var id) 
                ? id 
                : throw new UnauthorizedAccessException("UserId claim is missing or invalid.");
        }
    }

    public Guid? UserIdOrNull
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim?.Value, out var id) ? id : null;
        }
    }

    public string? IpAddress => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    public string? UserAgent => httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
}