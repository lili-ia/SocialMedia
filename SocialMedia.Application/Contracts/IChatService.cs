using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IChatService
{
    Task<Result<Chat>> CreateChat(CancellationToken ct);

    Task<Result<List<Message>>> GetMessagesByChatId(
        CancellationToken ct, 
        Guid chatId,
        int skipCount = 0,
        int pageSize = 10);

    Task<Result<List<ChatDto>>> GetAllChats(Guid userId, CancellationToken ct);
}