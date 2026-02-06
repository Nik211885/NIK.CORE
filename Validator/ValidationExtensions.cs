using System.Text.RegularExpressions;

namespace NIK.CORE.DOMAIN.Validator;

/// <summary>
/// Provides validation extension methods for <see cref="RuleBuilder{T, TValue}"/>.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// String validation extensions.
    /// </summary>
    extension<T>(RuleBuilder<T, string> ruleBuilderForString)
    {
        /// <summary>
        /// Validates that the string is not null, empty, or whitespace.
        /// </summary>
        /// <param name="message">Validation message when the value is empty.</param>
        /// <returns>The current <see cref="RuleBuilder{T, string}"/>.</returns>
        public RuleBuilder<T, string> NotEmpty(string message = CoreMessages.NotEmpty) 
            => ruleBuilderForString.Must(s => !string.IsNullOrWhiteSpace(s), message);

        /// <summary>
        /// Validates that the string is a valid email address format.
        /// </summary>
        /// <param name="message">Validation message when the email is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, string}"/>.</returns>
        public RuleBuilder<T, string> EmailAddress(string message = CoreMessages.InvalidEmailAddress) 
            => ruleBuilderForString.Must(
                s => !string.IsNullOrEmpty(s) && Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
                message
            );

        /// <summary>
        /// Validates that the string is a valid phone number.
        /// </summary>
        /// <param name="message">Validation message when the phone number is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, string}"/>.</returns>
        public RuleBuilder<T, string> PhoneNumber(string message = CoreMessages.InvalidPhoneNumber)  
            => ruleBuilderForString.Must(
                s => !string.IsNullOrEmpty(s) && Regex.IsMatch(s, @"^(0|84)(3|5|7|8|9|1[2|6|8|9])([0-9]{8})$"),
                message
            );

        /// <summary>
        /// Validates that the string meets password complexity requirements.
        /// </summary>
        /// <param name="message">Validation message when the password is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, string}"/>.</returns>
        public RuleBuilder<T, string> Password(string message = CoreMessages.InvalidPassword) 
            => ruleBuilderForString.Must(
                s => !string.IsNullOrEmpty(s) &&
                     Regex.IsMatch(s, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"),
                message
            );
    }

    /// <summary>
    /// Integer validation extensions.
    /// </summary>
    extension<T>(RuleBuilder<T, int> ruleBuilderForInt)
    {
        /// <summary>
        /// Validates that the integer value is within the specified age range.
        /// </summary>
        /// <param name="min">Minimum allowed age.</param>
        /// <param name="max">Maximum allowed age.</param>
        /// <param name="message">Validation message when the value is out of range.</param>
        /// <returns>The current <see cref="RuleBuilder{T, int}"/>.</returns>
        public RuleBuilder<T, int> AgeRange(int min = 18, int max = 100, string message = CoreMessages.MustRangeAge) 
            => ruleBuilderForInt.Must(i => i >= min && i <= max, string.Concat(message, $"{min} - {max}"));

        /// <summary>
        /// Validates that the integer value is positive.
        /// </summary>
        /// <param name="message">Validation message when the value is not positive.</param>
        /// <returns>The current <see cref="RuleBuilder{T, int}"/>.</returns>
        public RuleBuilder<T, int> Positive(string message = "") 
            => ruleBuilderForInt.Must(i => i > 0, CoreMessages.MustPositiveNumber);

        /// <summary>
        /// Validates that the integer value is negative.
        /// </summary>
        /// <param name="message">Validation message when the value is not negative.</param>
        /// <returns>The current <see cref="RuleBuilder{T, int}"/>.</returns>
        public RuleBuilder<T, int> Negative(string message = CoreMessages.MustNegativeNumber) 
            => ruleBuilderForInt.Must(i => i < 0, message);
    }

    /// <summary>
    /// DateTime validation extensions.
    /// </summary>
    extension<T>(RuleBuilder<T, DateTime> ruleBuilderForDateTime)
    {
        /// <summary>
        /// Validates that the date is not in the future.
        /// </summary>
        /// <param name="message">Validation message when the date is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, DateTime}"/>.</returns>
        public RuleBuilder<T, DateTime> IsBirthday(string message = CoreMessages.InvalidBirthdayDate) 
            => ruleBuilderForDateTime.Must(d => d <= DateTime.Now, message);

        /// <summary>
        /// Validates that the date is in the current year.
        /// </summary>
        /// <param name="message">Validation message when the year is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, DateTime}"/>.</returns>
        public RuleBuilder<T, DateTime> InCurrentYear(string message = CoreMessages.MustCurrentYear) 
            => ruleBuilderForDateTime.Must(d => d.Year == DateTime.Now.Year, message);

        /// <summary>
        /// Validates that the date is in the current month and year.
        /// </summary>
        /// <param name="message">Validation message when the month is invalid.</param>
        /// <returns>The current <see cref="RuleBuilder{T, DateTime}"/>.</returns>
        public RuleBuilder<T, DateTime> InCurrentMonth(string message = CoreMessages.MustCurrentMonth)
            => ruleBuilderForDateTime.Must(
                d => d.Month == DateTime.Now.Month && d.Year == DateTime.Now.Year,
                message
            );

        /// <summary>
        /// Validates that the date is today.
        /// </summary>
        /// <param name="message">Validation message when the date is not today.</param>
        /// <returns>The current <see cref="RuleBuilder{T, DateTime}"/>.</returns>
        public RuleBuilder<T, DateTime> InCurrentDay(string message = CoreMessages.MustCurrentDay)
            => ruleBuilderForDateTime.Must(d => d.Date == DateTime.Now.Date, message);
    }

    /// <summary>
    /// URI validation extensions.
    /// </summary>
    extension<T>(RuleBuilder<T, Uri> ruleBuilderForUri)
    {
        /// <summary>
        /// Validates that the URI points to an image resource.
        /// </summary>
        /// <param name="message">Validation message when the URI is not a valid image URL.</param>
        /// <returns>The current <see cref="RuleBuilder{T, Uri}"/>.</returns>
        public RuleBuilder<T, Uri> IsImageUrl(string message = CoreMessages.InvalidImageUri) 
            => ruleBuilderForUri.Must(
                u => Regex.IsMatch(u.ToString(), @"\.(jpg|jpeg|png|webp|gif)$", RegexOptions.IgnoreCase),
                message
            );
    }

    /// <summary>
    /// DateTimeOffset validation extensions.
    /// </summary>
    extension<T>(RuleBuilder<T, DateTimeOffset> ruleBuilderForDateTimeOffset)
    {
        /// <summary>
        /// Validates that the date and time have not expired.
        /// </summary>
        /// <param name="message">Validation message when the time is expired.</param>
        /// <returns>The current <see cref="RuleBuilder{T, DateTimeOffset}"/>.</returns>
        public RuleBuilder<T, DateTimeOffset> NotExpired(string message = CoreMessages.ExpiredTime) 
            => ruleBuilderForDateTimeOffset.Must(dto => dto > DateTimeOffset.Now, message);
    }
}
