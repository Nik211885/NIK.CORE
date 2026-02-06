namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Defines a validator capable of validating an instance of a specific type.
/// </summary>
/// <typeparam name="T">
/// The type of object to validate.
/// </typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Asynchronously validates the specified request.
    /// </summary>
    /// <param name="request">
    /// The instance to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationResult"/> containing validation errors, if any.
    /// </returns>
    Task<ValidationResult> ValidateAsync(T request);

    /// <summary>
    /// Synchronously validates the specified request.
    /// </summary>
    /// <param name="request">
    /// The instance to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationResult"/> containing validation errors, if any.
    /// </returns>
    ValidationResult Validate(T request);
}
