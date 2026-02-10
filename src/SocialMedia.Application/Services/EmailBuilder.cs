using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Services;

public class EmailBuilder : IEmailBuilder
{
    public string BuildEmailVerificationBody(string username, string verificationLink)
    {
        return $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; color: #333;"">
            <h2 style=""color: #4A90E2;"">Welcome to SocialMedia!</h2>
            <p>Hi there, {username}</p>
            <p>We're excited to have you join our community. Before you can start posting and following others, we just need to verify that this email belongs to you.</p>
            
            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{verificationLink}"" 
                   style=""background-color: #4A90E2; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                   Verify Email Address
                </a>
            </div>

            <p style=""font-size: 0.9em; color: #666;"">
                Or copy and paste this link into your browser:<br>
                <a href=""{verificationLink}"" style=""color: #4A90E2;"">{verificationLink}</a>
            </p>

            <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"">
            <p style=""font-size: 0.8em; color: #999;"">
                If you didn't create an account, you can safely ignore this email. 
                This link will expire in 24 hours.
            </p>
        </div>";
    }

    public string BuildPasswordResetBody(string username, string resetLink)
    {
        return $@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; color: #333;"">
            <h2 style=""color: #4A90E2;"">Welcome to SocialMedia!</h2>
            <p>Hi there, {username}</p>
            <p>You requested password reset for this account.</p>
            
            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{resetLink}"" 
                   style=""background-color: #4A90E2; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
                   Verify Email Address
                </a>
            </div>

            <p style=""font-size: 0.9em; color: #666;"">
                Or copy and paste this link into your browser:<br>
                <a href=""{resetLink}"" style=""color: #4A90E2;"">{resetLink}</a>
            </p>

            <hr style=""border: none; border-top: 1px solid #eee; margin: 20px 0;"">
            <p style=""font-size: 0.8em; color: #999;"">
                If you didn't request a password reset for this account, you can safely ignore this email. 
                This link will expire in 1 hour.
            </p>
        </div>";
    }
}