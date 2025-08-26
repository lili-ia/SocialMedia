using FluentValidation;

namespace SocialMedia.Application.Posts.GetPublicOfUsername;

public class GetPublicPostsOfUsernameCommandValidator : AbstractValidator<GetPublicPostsOfUsernameCommand>
{
    public GetPublicPostsOfUsernameCommandValidator()
    {
        RuleFor(x => x.AuthorUsername)
            .NotEmpty().WithMessage("AuthorUsername must be provided.")
            .MaximumLength(50).WithMessage("AuthorUsername must not exceed 50 characters.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be at most 100.");
    }
}