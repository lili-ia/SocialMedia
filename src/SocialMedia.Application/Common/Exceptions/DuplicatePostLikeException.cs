namespace SocialMedia.Application.Common.Exceptions;

public sealed class DuplicatePostLikeException(string? constraintName = null)
    : DuplicateEntityException("PostLike", constraintName, "User already liked this post.");