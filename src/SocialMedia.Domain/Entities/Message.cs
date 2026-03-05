using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid ChatId { get; private set; }
    
    public Guid SenderId { get; private set; }
    
    public string? Content { get; private set; }
    
    public MessageStatus Status { get; private set; } 
    
    public Guid? ParentMessageId { get; private set; }

    private readonly List<MessageAttachment> _attachments = [];
    public IReadOnlyList<MessageAttachment> Attachments => _attachments.AsReadOnly();

    public Chat Chat { get; private set; } = null!;
    public User Sender { get; private set; } = null!;
    public Message? ParentMessage { get; private set; }

    private Message() { }

    private Message(Guid chatId, Guid senderId, string? content, Guid? parentMessageId)
    {
        ChatId = chatId;
        SenderId = senderId;
        Content = content;
        Status = MessageStatus.Sent;
        ParentMessageId = parentMessageId;
    }

    public static Message Create(Guid chatId, Guid senderId, string? content, Guid? parentMessageId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainValidationException("Message content cannot be empty.");
        }

        var message = new Message(chatId, senderId, content, parentMessageId);
        
        message.AddDomainEvent(new MessageSentEvent(message.Id, chatId, senderId));

        return message;
    }

    public void AddAttachments(IEnumerable<MessageAttachment> attachments)
    {
        _attachments.AddRange(attachments.ToList());
    }

    public static Message CreateWithAttachments(
        Guid chatId, 
        Guid senderId, 
        string? content,
        IEnumerable<MessageAttachment> attachments,
        Guid? parentMessageId = null)
    {
        var attachmentList = attachments.ToList();
        
        if (string.IsNullOrWhiteSpace(content) && !attachmentList.Any())
        {
            throw new DomainValidationException("Message must have text or at least one attachment.");
        }

        var message = new Message(chatId, senderId, content, parentMessageId);

        message._attachments.AddRange(attachmentList);
        message.AddDomainEvent(new MessageSentEvent(message.Id, chatId, senderId));

        return message;
    }

    public void Delete(Guid requesterId)
    {
        if (SenderId != requesterId)
        {
            throw new DomainForbiddenException("You can only delete your own messages.");
        }

        Status = MessageStatus.Deleted;
        AddDomainEvent(new MessageDeletedEvent(Id, ChatId, SenderId));
    }
}