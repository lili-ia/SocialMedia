using FluentValidation;

namespace SocialMedia.Application.Posts.GetPublicByUserId;

public class GetPublicPostsByUserIdCommandValidator : AbstractValidator<GetPublicPostsByUserIdCommand>
{
    public GetPublicPostsByUserIdCommandValidator()
    {
        RuleFor(x => x.AuthorUserId)
            .NotEmpty().WithMessage("AuthorUserId must be provided.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be at most 100.");
    }
}