using System;

namespace Trading.Analytics.Utils
{
    // ReSharper disable once InconsistentNaming
    public struct TZDateTime
    {
        public TZDateTime(DateTime dateTime, TimeZoneInfo timeZone)
        {
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), timeZone);
            this.TimeZone = timeZone;
        }

        public DateTime UniversalTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
    }
}