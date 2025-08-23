namespace SocialMedia.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public DateTime BirthDate { get; set; }
    
    public string RawPassword { get; set; } = null!;
}