using Domain.Events;

namespace SocialMedia.Application.Contracts;

public interface INotificationService
{
    Task NotifyPostLiked(PostLikedEvent eventData);
    
    Task NotifyUserFollowed(FollowedEvent eventData);
    
    Task NotifyMessageReceived(MessageReceivedEvent eventData);
    
    Task NotifyPostCommented(PostCommentedEvent eventData);
}