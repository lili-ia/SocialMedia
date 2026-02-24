using Domain.Events;

namespace Domain.Entities;

public sealed class PostLike : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PostId { get; private set; }

    public User User { get; private set; } = null!;
    public Post Post { get; private set; } = null!;

    private PostLike() { } 

    private PostLike(Guid postId, Guid likerId)
    {
        PostId = postId;
        UserId = likerId;
    }

    public static PostLike Create(
        Guid postId,
        Guid postAuthorId,
        Guid likerId,
        string likerUsername)
    {
        var like = new PostLike(postId, likerId);

        like.AddDomainEvent(new PostLikedEvent(likerId, likerUsername, postAuthorId, postId));

        return like;
    }
}