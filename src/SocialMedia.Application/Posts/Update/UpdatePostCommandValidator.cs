using FluentValidation;

namespace SocialMedia.Application.Posts.Update;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    private static readonly string[] AllowedExtensions = [".jpeg", ".jpg", ".png"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;
    
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("PostId must be provided.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must be provided.");
        
        RuleFor(x => x)
            .Must(x => 
                !string.IsNullOrWhiteSpace(x.Text) || x.NewFiles is { Count: > 0 } || x.KeptStorageKeys is { Count: >0 })
            .WithMessage("Either Text, KeptStorageKeys or NewFiles must be provided.");
        
        When(x => !string.IsNullOrWhiteSpace(x.Text), () =>
        {
            RuleFor(x => x.Text)
                .MaximumLength(2000).WithMessage("Text max length must not exceed 2000 characters.");
        });
        
        When(x => x.NewFiles is { Count: > 0 }, () =>
        {
            RuleForEach(x => x.NewFiles).ChildRules(file =>
            {
                file.RuleFor(f => f.FileName)
                    .Must(f => AllowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .WithMessage(f => $"File '{f.FileName} must be a photo or gif (.jpeg, .jpg, .png, .gif).'");
                
                file.RuleFor(f => f.Content.Length)
                    .LessThanOrEqualTo(MaxFileSizeBytes)
                    .WithMessage(f => $"File '{f.FileName}' exceeds the maximum allowed size of 5 MB.");
            });
        });
    }
}