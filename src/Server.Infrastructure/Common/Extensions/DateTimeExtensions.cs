using NodaTime;

namespace HsaLedger.Server.Infrastructure.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ConvertToTimeZoneFromUtc(this DateTime utcDateTime, string timeZone)
    {
        var expectedTimeZone = DateTimeZoneProviders.Tzdb[timeZone];
        return Instant.FromDateTimeUtc(utcDateTime)
            .InZone(expectedTimeZone)
            .ToDateTimeUnspecified();
    }
}