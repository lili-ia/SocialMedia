using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Services;

public class ChatService : IChatService
{
    private readonly SocialMediaContext _db;
    private readonly ILogger<ChatService> _logger;
    private readonly IMapper _mapper;
    
    public ChatService(SocialMediaContext db, ILogger<ChatService> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<Result<Chat>> CreateChat(CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            IsGroup = false
        };

        try
        {
            await _db.Chats.AddAsync(chat, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a chat.");
            
            return Result<Chat>.FailureResult(
                $"An error occurred while creating this chat: {e.Message}", ErrorType.ServerError);
        }

        return Result<Chat>.SuccessResult(chat);
    }

    public async Task<Result<List<Message>>> GetMessagesByChatId(
        CancellationToken cancellationToken,
        int chatId,  
        int skipCount = 0,
        int pageSize = 10)
    {
        try
        {
            var messages = await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.Timestamp)
                .Skip(skipCount)
                .Take(pageSize)
                .Order()
                .ToListAsync(cancellationToken);

            return Result<List<Message>>.SuccessResult(messages);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving a chat.");
            
            return Result<List<Message>>.FailureResult(
                $"An error occurred while creating this chat: {e.Message}", ErrorType.ServerError);
        }
    }

    public async Task<Result<List<ChatDto>>> GetAllChats(int userId, CancellationToken cancellationToken)
    {
        
        var user = await _db.Users.FindAsync(userId, cancellationToken);
            
        if (user == null)
        {
            return Result<List<ChatDto>>.FailureResult($"Couldn`t find a user with id {userId}", ErrorType.NotFound);
        }

        try
        {
            var chats = await _db.Messages
                .Where(m => m.SenderId == userId)
                .Select(m => m.Chat)
                .Distinct()
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Select(chat => new 
                {
                    Chat = chat,
                    LastMessage = chat.Messages
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefault()
                })
                .Select(x => new ChatDto
                {
                    ChatId = x.Chat.ChatId,
                    LastMessageContent = x.LastMessage.Content,
                    LastMessageUser = x.LastMessage.Sender.Username,
                    LastMessageFromMe = x.LastMessage.Sender.UserId == userId
                })
                .ToListAsync(cancellationToken);

            return Result<List<ChatDto>>.SuccessResult(chats);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving all chats.");
            return Result<List<ChatDto>>.FailureResult(
                $"An error occurred while retrieving all chats: {e.Message}", ErrorType.ServerError);
        }
    }
}