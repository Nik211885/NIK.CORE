namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Represents a validation rule that can be executed against a specific instance type.
/// </summary>
/// <typeparam name="T">
/// The type of object to validate.
/// </typeparam>
public interface IValidationRule<in T>
{
    /// <summary>
    /// Validates the specified instance and appends validation results.
    /// </summary>
    /// <param name="instance">
    /// The instance to validate.
    /// </param>
    /// <param name="result">
    /// The validation result used to collect validation errors.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous validation operation.
    /// </returns>
    Task ValidateAsync(T instance, ValidationResult result);
}
