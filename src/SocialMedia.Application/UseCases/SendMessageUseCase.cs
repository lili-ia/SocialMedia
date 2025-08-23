using Domain.Entities;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;

namespace SocialMedia.Application.UseCases;

public class SendMessageUseCase : ISendMessageUseCase
{
    private readonly SocialMediaDbContext _db;
    private readonly ILogger<SendMessageUseCase> _logger;
    private readonly IChatService _chatService;
    
    public SendMessageUseCase(SocialMediaDbContext db, ILogger<SendMessageUseCase> logger, IChatService chatService)
    {
        _db = db;
        _logger = logger;
        _chatService = chatService;
    }
    
    public async Task<Message> ExecuteAsync(Guid chatId, string content, Guid senderId, CancellationToken ct)
    {
        var chat = await _db.Chats.FindAsync(chatId);

        if (chat == null)
        {
            var result = await _chatService.CreateChatAsync(ct);
            
            if (result.Success)
            {
                chatId = result.Value.Id;
            }
        }
        var message = new Message()
        {
            ChatId = chatId, 
            SenderId = senderId,
            Content = content,
            Timestamp = DateTime.UtcNow,
            IsRead = false,
            IsEdited = false,
        };

        try
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured while trying to send a message", e);
        }
        
        return message;
    }
}