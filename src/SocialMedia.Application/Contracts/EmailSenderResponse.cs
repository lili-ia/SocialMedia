namespace SocialMedia.Application.Contracts;

public sealed record EmailSenderResponse(bool IsSuccess, string? ErrorMessage);