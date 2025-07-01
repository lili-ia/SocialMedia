namespace SocialMedia.Application.DTOs;

public record ResetPasswordRequest (string Email, string Token, string NewPassword);