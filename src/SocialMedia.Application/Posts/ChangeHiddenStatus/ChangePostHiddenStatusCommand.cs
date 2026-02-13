using MediatR;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;

namespace SocialMedia.Application.Posts.ChangeHiddenStatus;

public sealed record ChangePostHiddenStatusCommand(
    Guid PostId, 
    Guid UserId, 
    bool MustBeHidden) : IRequest<Result>;