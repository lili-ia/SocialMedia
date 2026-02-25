using MediatR;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.DTOs.Block;

namespace SocialMedia.Application.Blocks.Create;

public sealed record CreateBlockCommand(
    Guid BlockerId,
    Guid BlockedId) : IRequest<Result<BlockResponse>>;