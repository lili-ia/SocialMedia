namespace Domain.Events;

public sealed class PostLikedEvent : DomainEvent
{
    public Guid LikerId { get; }
    
    public string LikerUsername { get; }
    
    public Guid ToUserId { get; }
    
    public Guid PostId { get; }

    public PostLikedEvent(
        Guid likerId,
        string likerUsername,
        Guid toUserId,
        Guid postId)
    {
        LikerId = likerId;
        LikerUsername = likerUsername;
        ToUserId = toUserId;
        PostId = postId;
    }
}