namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// Exception thrown when the server cannot or will not process the request 
/// due to something that is perceived to be a client error.
/// </summary>
/// <remarks>
/// Corresponds to HTTP Status Code 400 (Bad Request).
/// Uses C# 12 Primary Constructor syntax to pass the message and status code to <see cref="CustomException"/>.
/// </remarks>
/// <param name="message">
/// The error message that explains the reason for the exception. 
/// Defaults to <see cref="CoreMessages.BadRequest"/> if no message is provided.
/// </param>
public class BadRequestException(string message = CoreMessages.BadRequest) 
    : CustomException(message, 400);
