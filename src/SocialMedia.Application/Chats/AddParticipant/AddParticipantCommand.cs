using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Chats.AddParticipant;

public record AddParticipantCommand(
    Guid RequesterId,
    Guid ChatId,
    Guid NewUserId) : IRequest<Result>;