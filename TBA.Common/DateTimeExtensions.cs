using System;

namespace TBA.Common
{
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
    }
}