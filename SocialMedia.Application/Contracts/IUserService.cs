using Infrastructure.Models;
using SocialMedia.Application.DTOs;
using SocialMedia.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<User>> RegisterAsync(RegisterDto dto);
    
    Task<Result<string>> LoginAsync(LoginDto dto);

    Task<Result<User>> UpdateProfileAsync(UpdateUserDto dto, int userId);
}