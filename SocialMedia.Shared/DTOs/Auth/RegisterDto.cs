using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Shared.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = null!;
    
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress] 
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "RawPassword is required.")] 
    [StringLength(150, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 and at most 150.")] 
    public string RawPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "PasswordConfirm is required.")] 
    [StringLength(150, MinimumLength = 8, ErrorMessage = "Password length must be at least 8 and at most 150.")] 
    [Compare(nameof(RawPassword), ErrorMessage = "Passwords don't match.")]
    public string PasswordConfirm { get; set; } = null!;
}
     
