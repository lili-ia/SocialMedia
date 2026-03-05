using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IChatRepository
{
    Task<Chat?> GetByIdWithParticipantsAsync(Guid chatId, CancellationToken ct = default);
    
    Task<Chat?> GetDirectChatBetweenAsync(Guid userAId, Guid userBId, CancellationToken ct = default);
    
    Task<IReadOnlyList<TResult>> GetChatsForUserAsync<TResult>(
        Guid userId,
        Expression<Func<Chat, TResult>> selector,
        CancellationToken ct = default);
    
    Task AddAsync(Chat chat, CancellationToken ct = default);
    
    Task<bool> IsParticipantAsync(Guid chatId, Guid userId, CancellationToken ct = default);
}