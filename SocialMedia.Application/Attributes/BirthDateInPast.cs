using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Application.Attributes;

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