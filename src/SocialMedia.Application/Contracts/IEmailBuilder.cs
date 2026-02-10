namespace SocialMedia.Application.Contracts;

public interface IEmailBuilder
{
    string BuildEmailVerificationBody(string username, string verificationLink);

    string BuildPasswordResetBody(string username, string resetLink);
}