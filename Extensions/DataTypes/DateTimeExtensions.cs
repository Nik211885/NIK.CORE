namespace NIK.CORE.DOMAIN.Extensions.DataTypes;
/// <summary>
/// 
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dateTime"></param>
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Determines whether the DateTime equals <see cref="DateTime.MinValue"/>.
        /// </summary>
        bool IsMin() => dateTime == DateTime.MinValue;

        /// <summary>
        /// Determines whether the DateTime equals <see cref="DateTime.MaxValue"/>.
        /// </summary>
        bool IsMax() => dateTime == DateTime.MaxValue;
        /// <summary>
        /// Gets the start of the day (00:00:00).
        /// </summary>
        DateTime StartOfDay() => dateTime.Date;
        /// <summary>
        /// Gets the end of the day (23:59:59.9999999).
        /// </summary>
        DateTime EndOfDay() => dateTime.Date.AddDays(1).AddTicks(-1);
        /// <summary>
        /// Determines whether the DateTime is within the specified range (inclusive).
        /// </summary>
        bool IsBetween(DateTime from, DateTime to) => dateTime >= from && dateTime <= to;
        /// <summary>
        /// Converts the DateTime to UTC if it is not already UTC.
        /// </summary>
        DateTime ToUtc()
            => dateTime.Kind == DateTimeKind.Utc
                ? dateTime
                : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        /// <summary>
        /// Converts a UTC <see cref="DateTime"/> to the specified time zone.
        /// </summary>
        /// <param name="timeZoneId">
        /// The time zone identifier (e.g. "SE Asia Standard Time").
        /// </param>
        /// <returns>
        /// The converted local time in the specified time zone.
        /// </returns>
        DateTime ToTimeZone(string timeZoneId)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                return dateTime;

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
    }

    /// <summary>
    /// Provides extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTime">The DateTimeOffset value.</param>
    extension(DateTimeOffset dateTime)
    {
        /// <summary>
        /// Determines whether the value equals <see cref="DateTimeOffset.MinValue"/>.
        /// </summary>
        bool IsMin() => dateTime == DateTimeOffset.MinValue;
        /// <summary>
        /// Determines whether the value equals <see cref="DateTimeOffset.MaxValue"/>.
        /// </summary>
        bool IsMax() => dateTime == DateTimeOffset.MaxValue;
        /// <summary>
        /// Gets the start of the day (00:00:00) in the same offset.
        /// </summary>
        DateTimeOffset StartOfDay()
            => new(dateTime.Date, dateTime.Offset);
        /// <summary>
        /// Gets the end of the day (23:59:59.9999999) in the same offset.
        /// </summary>
        DateTimeOffset EndOfDay()
            => new(dateTime.Date.AddDays(1).AddTicks(-1), dateTime.Offset);
        /// <summary>
        /// Determines whether the value is within the specified range (inclusive).
        /// </summary>
        bool IsBetween(DateTimeOffset from, DateTimeOffset to)
            => dateTime >= from && dateTime <= to;
        /// <summary>
        /// Converts the value to UTC.
        /// </summary>
        DateTimeOffset ToUtc()
            => dateTime.ToUniversalTime();
        /// <summary>
        /// Converts a UTC <see cref="DateTimeOffset"/> to the specified time zone.
        /// </summary>
        /// <param name="timeZoneId">
        /// The time zone identifier (e.g. "SE Asia Standard Time").
        /// </param>
        /// <returns>
        /// The converted time in the specified time zone.
        /// </returns>
        DateTimeOffset ToTimeZone(string timeZoneId)
        {
            if (dateTime.Offset != TimeSpan.Zero)
                return dateTime;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }
    }
}
