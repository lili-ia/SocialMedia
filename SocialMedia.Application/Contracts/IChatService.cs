using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IChatService
{
    Task<Result<Chat>> CreateChatAsync(CancellationToken ct);

    Task<Result<List<Message>>> GetMessagesByChatIdAsync(
        CancellationToken ct, 
        Guid chatId,
        int skipCount = 0,
        int pageSize = 10);

    Task<Result<List<ChatDto>>> GetAllChatsAsync(Guid userId, CancellationToken ct);
}