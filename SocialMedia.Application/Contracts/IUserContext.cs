namespace SocialMedia.Application.Contracts;

public interface IUserContext
{
    Guid? UserId { get; }
}