using FluentValidation;

namespace SocialMedia.Application.Feed.GetFromFollowees;

public class GetFeedFromFolloweesCommandValidator : AbstractValidator<GetFeedFromFolloweesCommand>
{
    public GetFeedFromFolloweesCommandValidator()
    {
        
        RuleFor(x => x.ForUserId)
            .NotEmpty().WithMessage("ForUserId must be provided.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("PageSize must be at most 100.");
    }
}