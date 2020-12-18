using System;

namespace TBA.Common
{
    /// <summary>
    /// Extension methods for <seealso cref="DateTime"/> objects
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the DateTime as UTC based on the received Unix epoch time
        /// </summary>
        public static DateTime FromUnixEpochUtc(long millisecondsSinceUnixEpoch)
        {
            var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return utcEpoch.AddMilliseconds(millisecondsSinceUnixEpoch);
        }

        /// <summary>
        /// Returns the DateTime as UTC based on the received Unix epoch time
        /// </summary>
        public static DateTime FromUnixEpoch(long millisecondsSinceUnixEpoch)
        {
            var utcDateTime = FromUnixEpochUtc(millisecondsSinceUnixEpoch);
            var cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cstZone);
            return localDateTime;
        }

        /// <summary>
        /// Returns a date string in the date format for Tinybeans
        /// </summary>
        public static string ToTinybeansDateString(this DateTime source)
        {
            return source.ToString("EEEE dd MMMM yyyy");
        }

        /// <summary>
        /// Returns a date string in the month-year format for Tinybeans
        /// </summary>
        public static string ToTinybeansMonthYearString(this DateTime source)
        {
            return source.ToString("MMMM yyyy");
        }

        /// <summary>
        /// Returns a date string in the month format for Tinybeans
        /// </summary>
        public static string ToTinybeansMonthString(this DateTime source)
        {
            return source.ToString("MMMM");
        }

        /// <summary>
        /// Returns a date string in the YYYY-MM-dd format
        /// </summary>
        public static string ToIsoFormatString(this DateTime source)
        {
            return source.ToString("YYYY-MM-dd");
        }
    }
}