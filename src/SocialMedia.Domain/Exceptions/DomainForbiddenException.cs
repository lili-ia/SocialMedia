namespace Domain.Exceptions;

public class DomainForbiddenException(string message) : DomainException(message);