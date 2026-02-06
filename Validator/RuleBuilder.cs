using System.Linq.Expressions;
namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Builds and executes validation rules for a specific property of a target type.
/// </summary>
/// <typeparam name="T">
/// The type of object being validated.
/// </typeparam>
/// <typeparam name="TProp">
/// The type of the property being validated.
/// </typeparam>
public class RuleBuilder<T, TProp> : IValidationRule<T>
{
    private readonly Func<T, TProp> _valueGetter;
    private readonly string _propertyName;
    private readonly List<(Func<TProp, Task<bool>> Check, string Message)> _rules = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleBuilder{T, TProp}"/> class
    /// using the specified property expression.
    /// </summary>
    /// <param name="expression">
    /// An expression that identifies the property to validate.
    /// </param>
    public RuleBuilder(Expression<Func<T, TProp>> expression)
    {
        _valueGetter = expression.Compile();
        _propertyName = ((MemberExpression)expression.Body).Member.Name;
    }

    /// <summary>
    /// Adds a synchronous validation rule for the property.
    /// </summary>
    /// <param name="check">
    /// A predicate that determines whether the property value is valid.
    /// </param>
    /// <param name="message">
    /// The error message used when validation fails.
    /// </param>
    /// <returns>
    /// The current <see cref="RuleBuilder{T, TProp}"/> instance for chaining.
    /// </returns>
    public RuleBuilder<T, TProp> Must(Predicate<TProp> check, string message)
    {
        _rules.Add((val => Task.FromResult(check(val)), message));
        return this;
    }

    /// <summary>
    /// Adds an asynchronous validation rule for the property.
    /// </summary>
    /// <param name="checkAsync">
    /// An asynchronous function that determines whether the property value is valid.
    /// </param>
    /// <param name="message">
    /// The error message used when validation fails.
    /// </param>
    /// <returns>
    /// The current <see cref="RuleBuilder{T, TProp}"/> instance for chaining.
    /// </returns>
    public RuleBuilder<T, TProp> MustAsync(Func<TProp, Task<bool>> checkAsync, string message)
    {
        _rules.Add((checkAsync, message));
        return this;
    }

    /// <summary>
    /// Validates the specified instance against all configured rules
    /// for the associated property.
    /// </summary>
    /// <param name="instance">
    /// The instance being validated.
    /// </param>
    /// <param name="result">
    /// The validation result used to collect validation failures.
    /// </param>
    public async Task ValidateAsync(T instance, ValidationResult result)
    {
        var value = _valueGetter(instance);
        foreach (var (check, message) in _rules)
        {
            if (!await check(value))
            {
                result.Errors.Add(new ValidationFailure
                {
                    PropertyName = _propertyName,
                    ErrorMessage = message
                });
            }
        }
    }
}
