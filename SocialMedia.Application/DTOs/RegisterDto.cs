using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Username field is required.")] 
    public string Username { get; set; }
    
    [Required]
    [EmailAddress] 
    public string Email { get; set; }
    
    [Required] 
    [StringLength(30, MinimumLength = 6)]
    public string RawPassword { get; set; }
    
    [Required] 
    [StringLength(30, MinimumLength = 6)] 
    [Compare(nameof(RawPassword), ErrorMessage = "Passwords don`t match.")]
    public string PasswordConfirm { get; set; }
}
     
