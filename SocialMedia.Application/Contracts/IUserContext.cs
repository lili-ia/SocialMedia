namespace SocialMedia.Application.Contracts;

public interface IUserContext
{
    Guid? UserId { get; }
    
    string? IpAddress { get; }
    
    string? UserAgent { get; }
}