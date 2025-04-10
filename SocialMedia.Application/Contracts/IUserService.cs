
using Infrastructure;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<User>> RegisterAsync(RegisterDto dto);
    
    Task<Result<string>> LoginAsync(LoginDto dto);
}