using FluentValidation;

namespace SocialMedia.Application.Posts.GetPublicOfUser;

public class GetPublicPostsOfUserCommandValidator : AbstractValidator<GetPublicPostsOfUserCommand>
{
    public GetPublicPostsOfUserCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => (x.AuthorId.HasValue && string.IsNullOrEmpty(x.AuthorUsername)) ||
                       (!x.AuthorId.HasValue && !string.IsNullOrEmpty(x.AuthorUsername)))
            .WithMessage("Exactly one of AuthorId or AuthorUsername must be provided.");

        When(x => !string.IsNullOrEmpty(x.AuthorUsername), () =>
        {
            RuleFor(x => x.AuthorUsername)
                .MaximumLength(50).WithMessage("AuthorUsername must not exceed 50 characters.");
        });

        When(x => x.AuthorId.HasValue, () =>
        {
            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId must be provided.");
        });
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be at most 100.");
    }
}