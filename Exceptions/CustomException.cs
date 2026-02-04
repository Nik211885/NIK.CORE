namespace NIK.CORE.DOMAIN.Exceptions;

/// <summary>
/// Serves as the base class for all domain-specific exceptions within the NIK framework.
/// </summary>
/// <remarks>
/// This abstract class extends <see cref="Exception"/> to include a specific HTTP status code,
/// allowing global exception handlers to automatically map domain errors to correct HTTP responses.
/// </remarks>
/// <param name="message">The localized or technical message describing the error.</param>
/// <param name="statusCode">The HTTP status code associated with this error (e.g., 404, 400, 403).</param>
public abstract class CustomException(string message, int statusCode) : Exception(message)
{
    /// <summary>
    /// Gets the error message that explains the reason for the exception.
    /// </summary>
    public override string Message { get; } = message;

    /// <summary>
    /// Gets the HTTP status code intended for the API response.
    /// </summary>
    public int StatusCode { get; } = statusCode;
}
