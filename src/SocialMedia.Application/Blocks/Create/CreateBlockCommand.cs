using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Blocks.Create;

public sealed record CreateBlockCommand(
    Guid BlockerId,
    Guid BlockedId) : IRequest<Result<Guid>>;