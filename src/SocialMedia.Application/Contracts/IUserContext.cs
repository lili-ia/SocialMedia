namespace SocialMedia.Application.Contracts;

public interface IUserContext
{
    Guid UserId { get; }
 
    Guid? UserIdOrNull { get; }
    
    string? IpAddress { get; }
    
    string? UserAgent { get; }
}