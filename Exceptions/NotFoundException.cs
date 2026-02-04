namespace NIK.CORE.DOMAIN.Exceptions;
/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404).
/// </summary>
public class NotFoundException(string message = CoreMessages.NotFound) 
    : CustomException(message, 404);
