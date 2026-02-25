using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Blocks.Delete;

public sealed record DeleteBlockCommand(
    Guid BlockerId, 
    Guid BlockedId) : IRequest<Result<MessageResponse>>;