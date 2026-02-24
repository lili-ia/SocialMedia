namespace Domain.Exceptions;

public class RateLimitDomainException(string message) : DomainException(message) { }