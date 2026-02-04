namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// Exception thrown when a user is authenticated but does not have the 
/// necessary permissions to access a specific resource or perform an action.
/// </summary>
/// <remarks>
/// Maps to HTTP Status Code 401 (Unauthorized). 
/// This signifies a failure in the authorization layer of the application.
/// </remarks>
/// <param name="message">
/// The error message explaining why access was denied. 
/// Defaults to <see cref="CoreMessages.PermissionDenied"/> if not provided.
/// </param>
public class PermissionDeniedException(string message = CoreMessages.PermissionDenied)
    : CustomException(message, 401);
