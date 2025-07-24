using Domain.Enums;

namespace SocialMedia.Shared.DTOs.Auth;

public record class RegisterResponse
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public UserStatus Status { get; set; } = UserStatus.Pending;
    
    public UserRole UserRole { get; set; } = UserRole.User;
}