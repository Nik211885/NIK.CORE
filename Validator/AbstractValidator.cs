using System.Linq.Expressions;
namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Base class for defining validation rules for a specific type.
/// </summary>
/// <typeparam name="T">
/// The type of object being validated.
/// </typeparam>
public abstract class AbstractValidator<T> : IValidator<T>
{
    private readonly List<IValidationRule<T>> _rules = [];

    /// <summary>
    /// Creates a validation rule for a specific property of the target type.
    /// </summary>
    /// <typeparam name="TProp">
    /// The type of the property being validated.
    /// </typeparam>
    /// <param name="expression">
    /// An expression that specifies the property to validate.
    /// </param>
    /// <returns>
    /// A <see cref="RuleBuilder{T, TProp}"/> used to configure validation rules.
    /// </returns>
    protected RuleBuilder<T, TProp> RuleFor<TProp>(Expression<Func<T, TProp>> expression)
    {
        var rule = new RuleBuilder<T, TProp>(expression);
        _rules.Add(rule);
        return rule;
    }

    /// <summary>
    /// Asynchronously validates the specified instance against all configured rules.
    /// </summary>
    /// <param name="instance">
    /// The instance to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationResult"/> containing validation errors, if any.
    /// </returns>
    public async Task<ValidationResult> ValidateAsync(T instance)
    {
        var result = new ValidationResult();
        foreach (var rule in _rules)
        {
            await rule.ValidateAsync(instance, result);
        }
        return result;
    }

    /// <summary>
    /// Synchronously validates the specified instance against all configured rules.
    /// </summary>
    /// <param name="instance">
    /// The instance to validate.
    /// </param>
    /// <returns>
    /// A <see cref="ValidationResult"/> containing validation errors, if any.
    /// </returns>
    public ValidationResult Validate(T instance)
    {
        var result = new ValidationResult();
        foreach (var rule in _rules)
        {
            rule.ValidateAsync(instance, result).GetAwaiter().GetResult();
        }
        return result;
    }
}
