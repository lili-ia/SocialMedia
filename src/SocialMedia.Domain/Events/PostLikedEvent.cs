namespace Domain.Events;

public sealed class PostLikedEvent : DomainEvent
{
    public Guid FromUserId { get; }
    public string FromUsername { get; }
    public Guid ToUserId { get; }
    public Guid PostId { get; }

    public PostLikedEvent(
        Guid fromUserId,
        string fromUsername,
        Guid toUserId,
        Guid postId)
    {
        FromUserId = fromUserId;
        FromUsername = fromUsername;
        ToUserId = toUserId;
        PostId = postId;
    }
}