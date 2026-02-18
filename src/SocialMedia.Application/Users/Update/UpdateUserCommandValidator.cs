using FluentValidation;

namespace SocialMedia.Application.Users.Update;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] AllowedExtensions = [".jpeg", ".jpg", ".png"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    private const int MinimumAge = 13;
    
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId must be provided.");
        
        RuleFor(x => x)
            .Must(x => 
                !string.IsNullOrWhiteSpace(x.Bio) 
                || x.ProfilePic is not null 
                || x.BirthDate is null)
            .WithMessage("At least one field (Bio, ProfilePic, or BirthDate) must be provided.");

        RuleFor(x => x.Bio)
            .MaximumLength(300)
            .When(x => x.Bio is not null);
        
        When(x => x.ProfilePic is not null, () =>
        {
            RuleFor(x => x.ProfilePic!)
                .ChildRules(file =>
                {
                    file.RuleFor(f => f.FileName)
                        .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLower()))
                        .WithMessage("File must be a photo (.jpeg, .jpg, .png).");
                    
                    file.RuleFor(f => f.Content.Length)
                        .LessThanOrEqualTo(MaxFileSizeBytes)
                        .WithMessage("File exceeds the maximum allowed size of 5 MB.");
                });
        });
        
        When(x => x.BirthDate is not null, () =>
        {
            RuleFor(x => x.BirthDate)
                .Must(date => date!.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("BirthDate cannot be in the future.")
                .Must(date => date!.Value <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-MinimumAge))
                .WithMessage($"User must be at least {MinimumAge} years old.");
        });
    }
}