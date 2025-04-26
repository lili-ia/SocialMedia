using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Contracts;

public interface IChatService
{
    Task<Result<Chat>> CreateChat(CancellationToken cancellationToken);

    Task<Result<List<Message>>> GetMessagesByChatId(
        CancellationToken cancellationToken, 
        int chatId,
        int skipCount = 0,
        int pageSize = 10);

    Task<Result<List<ChatDto>>> GetAllChats(int userId, CancellationToken cancellationToken);
}