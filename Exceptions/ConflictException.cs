namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// Exception thrown when a request conflicts with the current state of the server.
/// </summary>
/// <remarks>
/// Corresponds to HTTP Status Code 409 (Conflict). 
/// This is typically used for data concurrency issues or duplicate entries (e.g., trying to register an email that already exists).
/// </remarks>
/// <param name="message">
/// The error message that explains the reason for the conflict. 
/// Defaults to <see cref="CoreMessages.Conflict"/> if no specific message is provided.
/// </param>
public class ConflictException(string message = CoreMessages.Conflict) 
    : CustomException(message, 409);
