using FluentValidation;

namespace SocialMedia.Application.Comments.GetAllForPost;

public class GetAllCommentsForPostCommandValidator : AbstractValidator<GetAllCommentsForPostCommand>
{
    public GetAllCommentsForPostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("PostId must be provided.");
        
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("PostId must be provided.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be at most 100.");
    }
}