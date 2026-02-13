namespace SocialMedia.Application.Common.Exceptions;

public class FileStorageException(string message, Exception? inner) 
    : Exception(message, inner);