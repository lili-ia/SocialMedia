using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Chats.Leave;

public sealed record LeaveChatCommand(Guid ChatId, Guid UserId) : IRequest<Result>;