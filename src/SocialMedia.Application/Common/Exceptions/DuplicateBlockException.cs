namespace SocialMedia.Application.Common.Exceptions;

public sealed class DuplicateBlockException(string? constraintName = null)
    : DuplicateEntityException("Block", constraintName, "Block already exists.");