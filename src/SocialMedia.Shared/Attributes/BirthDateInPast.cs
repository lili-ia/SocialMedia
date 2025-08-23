using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Shared.Attributes;

public class BirthDateInPast : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date < DateTime.Today;
        }

        return false;
    }
}