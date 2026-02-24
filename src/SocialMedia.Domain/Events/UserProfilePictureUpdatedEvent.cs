namespace Domain.Events;

public class UserProfilePictureUpdatedEvent : DomainEvent
{
    public UserProfilePictureUpdatedEvent(Guid userId, Guid profilePictureId)
    {
        UserId = userId;
        ProfilePictureId = profilePictureId;
    }

    public Guid UserId { get; }
    
    public Guid ProfilePictureId { get; }
}