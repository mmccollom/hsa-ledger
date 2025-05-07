using NodaTime;

namespace HsaLedger.Shared.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundDown(this DateTime dt, int minutes)
    {
        dt = dt.AddSeconds(-1 * dt.Second);
        while (true)
        {
            if (dt.Minute % minutes == 0)
                break;
            
            dt = dt.AddMinutes(-1);
        }

        return dt;
    }
    
    public static DateTime RoundUp(this DateTime dt, int minutes)
    {
        dt = dt.AddSeconds(-1 * dt.Second);
        while (true)
        {
            if (dt.Minute % minutes == 0)
                break;
            
            dt = dt.AddMinutes(1);
        }

        return dt;
    }
    
    public static DateTime Nearest(this DateTime dt, int minutes)
    {
        var up = dt.RoundUp(minutes);
        var down = dt.RoundDown(minutes);
        var diffUp = up - dt;
        var diffDown = dt - down;
        
        return diffUp < diffDown ? up : down;
    }

    // ReSharper disable once InconsistentNaming
    public static int UtcOffsetInHours(this string timeZone)
    {
        var zone = DateTimeZoneProviders.Tzdb[timeZone];
        var offset = zone.GetUtcOffset(SystemClock.Instance.GetCurrentInstant());
        return offset.Milliseconds / NodaConstants.MillisecondsPerHour;
    }
    
    // ReSharper disable once InconsistentNaming
    public static DateTime ConvertToTimeZoneFromUtc(this DateTime utcDateTime, string timeZone)
    {
        var expectedTimeZone = DateTimeZoneProviders.Tzdb[timeZone];
        return Instant.FromDateTimeUtc(utcDateTime)
            .InZone(expectedTimeZone)
            .ToDateTimeUnspecified();
    }

    public static DateTime ConvertFromTimezoneToUtc(this DateTime datetime, string timeZone)
    {
        var localDateTime = LocalDateTime.FromDateTime(datetime);
        var expectedTimeZone = DateTimeZoneProviders.Tzdb[timeZone];
        var zonedDateTime = localDateTime.InZoneStrictly(expectedTimeZone);
        var utcDateTime = zonedDateTime.WithZone(DateTimeZone.Utc);
        return utcDateTime.ToDateTimeUtc();
    }
    
    public static List<DateTime> Range(this DateTime start, DateTime end, bool includeEndDate)
    {
        var range = new List<DateTime>();
        var current = start;
        while (current <= end)
        {
            range.Add(current);
            current = current.AddDays(1);
        }

        if (includeEndDate)
        {
            range.Add(end);
        }

        return range;
    }
}

