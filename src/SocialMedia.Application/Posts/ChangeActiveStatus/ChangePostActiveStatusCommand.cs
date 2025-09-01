using MediatR;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Posts.ChangeActiveStatus;

public sealed record ChangePostActiveStatusCommand(
    Guid PostId, 
    Guid UserId, 
    bool ActiveStatus) : IRequest<Result<Guid>>;