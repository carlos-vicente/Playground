using System;

namespace Playground.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime GetToTheMillisecond(this DateTime current)
        {
            return new DateTime(
                current.Year,
                current.Month,
                current.Day,
                current.Hour,
                current.Minute,
                current.Second,
                current.Millisecond);
        }
    }
}