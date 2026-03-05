using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Messages.Delete;

public record DeleteMessageCommand(Guid RequesterId, Guid MessageId) : IRequest<Result>;