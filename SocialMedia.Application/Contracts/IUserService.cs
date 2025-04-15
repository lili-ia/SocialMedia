using Domain.Entities;
using SocialMedia.Application.DTOs;
using SocialMedia.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IUserService
{
    Task<Result<User>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
    
    Task<Result<string>> LoginAsync(LoginDto dto, CancellationToken cancellationToken);

    Task<Result<User>> UpdateProfileAsync(UpdateUserDto dto, int userId, CancellationToken cancellationToken);
}