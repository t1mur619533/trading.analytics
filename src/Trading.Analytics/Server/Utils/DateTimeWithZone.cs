using System;

namespace Trading.Analytics.Server.Utils
{
    public struct DateTimeWithZone
    {
        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            UniversalTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            this.TimeZone = timeZone;
        }

        public DateTime UniversalTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DateTime LocalTime => TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
    }
}