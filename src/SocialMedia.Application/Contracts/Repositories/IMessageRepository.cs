using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid messageId, CancellationToken ct = default);
    
    Task<IReadOnlyList<TResult>> GetMessagesForChatAsync<TResult>(
        Guid chatId,
        Expression<Func<Message, TResult>> selector,
        int skip, int take,
        CancellationToken ct = default);
    
    Task AddAsync(Message message, CancellationToken ct = default);
}