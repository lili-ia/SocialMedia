using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Chat;

namespace SocialMedia.Application.Chats.GetMy;

public record GetMyChatsCommand(Guid UserId) : IRequest<Result<IReadOnlyList<ChatDto>>>;