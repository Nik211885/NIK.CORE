namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Represents a single validation failure for a specific property.
/// </summary>
public class ValidationFailure
{
    /// <summary>
    /// Gets or sets the name of the property that failed validation.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the validation error message.
    /// </summary>
    public string ErrorMessage { get; set; }
}

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <c>true</c> when no validation errors are present.
    /// </remarks>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the collection of validation failures.
    /// </summary>
    public List<ValidationFailure> Errors { get; } = [];
}
