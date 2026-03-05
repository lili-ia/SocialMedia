using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public class Chat : BaseEntity
{
    public string? Name { get; private set; }
    
    public ChatType Type { get; private set; }
    
    public Guid CreatorId { get; private set; }
    
    public Guid? LastMessageId { get; private set; }
    
    public Message? LastMessage { get; private set; }
    
    public DateTime LastActivityAt { get; private set; }

    public IReadOnlyList<ChatParticipant> Participants => _participants.AsReadOnly();
    
    public IReadOnlyList<Message> Messages => _messages.AsReadOnly();
    
    public User Creator { get; private set; } = null!;
    
     private Chat() { }

    public static Chat CreateDirect(Guid creatorId, Guid otherUserId)
    {
        var chat = new Chat
        {
            Type = ChatType.Direct,
            CreatorId = creatorId,
        };

        chat._participants.Add(ChatParticipant.Create(chat.Id, creatorId, isAdmin: false));
        chat._participants.Add(ChatParticipant.Create(chat.Id, otherUserId, isAdmin: false));

        chat.AddDomainEvent(new ChatCreatedEvent(chat.Id, creatorId, [creatorId, otherUserId])); // todo add event handler

        return chat;
    }

    public static Chat CreateGroup(Guid creatorId, string name, IEnumerable<Guid> participantIds)
    {
        var chat = new Chat
        {
            Type = ChatType.Group,
            Name = name,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        chat._participants.Add(ChatParticipant.Create(chat.Id, creatorId, isAdmin: true));

        foreach (var participantId in participantIds.Where(id => id != creatorId))
        {
            chat._participants.Add(ChatParticipant.Create(chat.Id, participantId, isAdmin: false));
        }

        var allIds = chat._participants.Select(p => p.UserId).ToList();
        chat.AddDomainEvent(new ChatCreatedEvent(chat.Id, creatorId, allIds));  // todo add event handler

        return chat;
    }

    public void AddParticipant(Guid requesterId, Guid newUserId)
    {
        EnsureGroupChat();
        EnsureAdmin(requesterId);

        if (_participants.Any(p => p.UserId == newUserId && p.IsActive))
        {
            throw new DomainConflictException("User is already a participant.");
        }

        _participants.Add(ChatParticipant.Create(Id, newUserId, isAdmin: false));
        AddDomainEvent(new ChatParticipantAddedEvent(Id, newUserId));
    }

    public void RemoveParticipant(Guid requesterId, Guid targetUserId)
    {
        EnsureGroupChat();
        EnsureAdmin(requesterId);

        var participant = _participants.FirstOrDefault(p => p.UserId == targetUserId && p.IsActive)
            ?? throw new DomainNotFoundExceptions("Participant not found.");

        participant.Deactivate();
    }

    public void Leave(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId && p.IsActive)
            ?? throw new DomainNotFoundExceptions("You are not a participant of this chat.");

        participant.Deactivate(); // todo domain event UserLeftChat
    }

    public bool IsParticipant(Guid userId)
        => _participants.Any(p => p.UserId == userId && p.IsActive);

    private void EnsureGroupChat()
    {
        if (Type != ChatType.Group)
        {
            throw new DomainForbiddenException("This action is only allowed in group chats.");
        }
    }

    private void EnsureAdmin(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId && p.IsActive);
        
        if (participant is null || !participant.IsAdmin)
        {
            throw new DomainForbiddenException("Only admins can perform this action.");
        }
    }
    
    private readonly List<ChatParticipant> _participants = [];
    private readonly List<Message> _messages = [];
}