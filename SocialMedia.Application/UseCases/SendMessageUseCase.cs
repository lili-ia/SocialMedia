using Domain.Entities;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.UseCases;

public class SendMessageUseCase : ISendMessageUseCase
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<SendMessageUseCase> _logger;
    private readonly IChatService _chatService;
    
    public SendMessageUseCase(SocialMediaContext db, ILogger<SendMessageUseCase> logger, IChatService chatService)
    {
        _db = db;
        _logger = logger;
        _chatService = chatService;
    }
    
    public async Task<Message> ExecuteAsync(int chatId, string content, int senderId, CancellationToken ct)
    {
        var chat = await _db.Chats.FindAsync(chatId);

        if (chat == null)
        {
            var result = await _chatService.CreateChat(ct);
            
            if (result.Success)
            {
                chatId = result.Value.ChatId;
            }
        }
        var message = new Message()
        {
            ChatId = chatId, 
            SenderId = senderId,
            Content = content,
            Timestamp = DateTime.Now,
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