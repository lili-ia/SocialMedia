namespace Domain.Enums;

public enum UserStatus 
{
    Active = 0,        // User is active and can use the app normally
    Banned = 1,        // User is banned and cannot access the app
    Suspended = 2,     // Temporarily suspended by moderators
    Pending = 3,       // Newly registered, waiting for email verification or approval
    Deactivated = 4    // User voluntarily deactivated their account
}