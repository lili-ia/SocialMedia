namespace SocialMedia.Application.Common.Exceptions;

public abstract class DuplicateEntityException(
    string entityName,
    string? constraintName = null,
    string? message = null)
    : Exception(message ?? $"Duplicate entity detected: {entityName}")
{
    public string EntityName { get; } = entityName;
    
    public string? ConstraintName { get; } = constraintName;
}