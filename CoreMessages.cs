namespace NIK.CORE.DOMAIN;

public static class CoreMessages
{
    // message for exception
    public const string BadRequest = "core.exception.bad_request";
    public const string Unauthorized = "core.exception.unauthorized_request";
    public const string PermissionDenied = "core.exception.permission_denied";
    public const string NotFound = "core.exception.not_found";
    public const string Conflict = "core.exception.conflict";
    
    // message for extension validation
    public const string NotEmpty = "core.validation.not_empty";
    public const string InvalidEmailAddress =  "core.validation.invalid_email_address";
    public const string InvalidPhoneNumber = "core.validation.invalid_phone_number";
    public const string InvalidPassword = "core.validation.invalid_password";
    public const string MustRangeAge = "core.validation.must_range_age";
    public const string MustPositiveNumber = "core.validation.must_positive_number";
    public const string MustNegativeNumber = "core.validation.must_negative_number";
    public const string InvalidBirthdayDate = "core.validation.invalid_birthday_date";
    public const string MustCurrentYear = "core.validation.must_current_year";
    public const string MustCurrentMonth = "core.validation.must_current_month";
    public const string MustCurrentDay = "core.validation.must_current_day";
    public const string ExpiredTime = "core.validation.expired_time";
    public const string InvalidImageUri = "core.validation.invalid_image_uri";
    
    // message for config
    public const string NotExitConfigKey = "core.config.not_exit_config_key";
    public const string ExitConfigKey = "core.config.exit_config_key";
    public const string ExitConfigValue = "core.config.exit_config_value";
    
}
