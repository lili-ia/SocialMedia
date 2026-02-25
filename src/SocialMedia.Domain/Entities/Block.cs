using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class Block : BaseEntity
{
    public Guid BlockerId { get; private set; }
    
    public Guid BlockedId { get; private set; }

    public string? Reason { get; private set; }

    public User Blocker { get; private set; } = null!;
    
    public User Blocked { get; private set; } = null!;

    private Block() { } 

    private Block(Guid blockerId, Guid blockedId, string? reason)
    {
        if (blockerId == blockedId)
        {
            throw new DomainConflictException("User cannot block themselves.");
        }

        BlockerId = blockerId;
        BlockedId = blockedId;
        Reason = reason;
    }

    public static Block Create(
        Guid blockerId,
        Guid blockedId,
        string? reason = null)
    {
        if (blockerId == blockedId)
        {
            throw new DomainForbiddenException("You can not block yourself.");
        }
        
        var block = new Block(blockerId, blockedId, reason);

        block.AddDomainEvent(new UserBlockedEvent(blockerId, blockedId));

        return block;
    }

    public void UpdateReason(string reason)
    {
        Reason = reason;
        MarkAsUpdated();
    }
}