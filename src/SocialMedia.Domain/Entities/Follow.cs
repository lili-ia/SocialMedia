using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class Follow : BaseEntity
{
    private Follow() { }

    private Follow(Guid followerId, Guid followeeId)
    {
        if (followerId == followeeId)
        {
            throw new DomainConflictException("User cannot follow themselves.");
        }

        FollowerId = followerId;
        FolloweeId = followeeId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid FollowerId { get; private set; }

    public Guid FolloweeId { get; private set; }

    public User Follower { get; private set; } = null!;

    public User Followee { get; private set; } = null!;

    public static Follow Create(Guid followerId, Guid followeeId)
    {
        var follow = new Follow(followerId, followeeId);
        
        follow.AddDomainEvent(new FollowedEvent(follow.Id, followerId, followeeId));

        return follow;
    }

    public override void SoftDelete()
    {
        base.SoftDelete();
        
        AddDomainEvent(new UnfollowedEvent(FollowerId, FolloweeId));
    }
}