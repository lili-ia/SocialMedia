using Domain.Events;

namespace SocialMedia.Application.Contracts;

public interface INotificationService
{
    Task NotifyPostLikedAsync(PostLikedEvent eventData);
    
    Task NotifyUserFollowedAsync(FollowedEvent eventData);
    
    Task NotifyMessageReceivedAsync(MessageReceivedEvent eventData);
    
    Task NotifyPostCommentedAsync(PostCommentedEvent eventData);
}