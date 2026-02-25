namespace SocialMedia.Application.Common.Exceptions;

public class DuplicateFollowException(string? constraintName = null)
    : DuplicateEntityException("Follow", constraintName, "Follow already exists.");