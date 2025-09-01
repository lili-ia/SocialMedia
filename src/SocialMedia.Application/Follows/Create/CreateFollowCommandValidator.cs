using FluentValidation;

namespace SocialMedia.Application.Follows.Create;

public class CreateFollowCommandValidator : AbstractValidator<CreateFollowCommand>
{
    public CreateFollowCommandValidator()
    {
        RuleFor(x => x.FolloweeId)
            .NotEmpty().WithMessage("FolloweeId must be provided.");

        RuleFor(x => x.FollowerId)
            .NotEmpty().WithMessage("FollowerId must be provided.");

        RuleFor(x => x)
            .Must(x => x.FolloweeId != x.FollowerId)
            .WithMessage("You cannot follow yourself.");
    }
}