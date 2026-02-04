namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// Exception thrown when a user is not authenticated (HTTP 401).
/// </summary>
public class UnAuthorizedException(string message = CoreMessages.Unauthorized) 
    : CustomException(message, 401);
