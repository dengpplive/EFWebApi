using System;

namespace YSL.Common.Extender
{
    /// <summary>
    /// DateTime 扩展
    /// </summary>
    public static class DateTimeExtension {
        public static bool IsWeekend(this DateTime date) {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        public static long GetUnixTime(this DateTime time) {
            return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
    }
}