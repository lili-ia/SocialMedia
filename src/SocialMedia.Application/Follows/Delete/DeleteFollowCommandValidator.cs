using FluentValidation;

namespace SocialMedia.Application.Follows.Delete;

public class DeleteFollowCommandValidator : AbstractValidator<DeleteFollowCommand>
{
    public DeleteFollowCommandValidator()
    {
        RuleFor(x => x.FolloweeId)
            .NotEmpty().WithMessage("FolloweeId must be provided.");

        RuleFor(x => x.FollowerId)
            .NotEmpty().WithMessage("FollowerId must be provided.");

        RuleFor(x => x)
            .Must(x => x.FolloweeId != x.FollowerId)
            .WithMessage("You cannot unfollow yourself.");
    }
}