using System;

namespace Elephanta.Application.Common;

public static class DateTimeConverter
{
    // Convert a DateTime representing IST to UTC
    public static DateTime ConvertIstToUtc(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc) return dt;

        var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        TimeZoneInfo istZone;
        try
        {
            istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        }
        catch
        {
            try
            {
                istZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
            }
            catch
            {
                // Fallback: subtract 5.5 hours
                return unspecified.AddHours(-5.5);
            }
        }

        return TimeZoneInfo.ConvertTimeToUtc(unspecified, istZone);
    }

    // Convert a UTC DateTime to IST
    public static DateTime ConvertUtcToIst(DateTime utc)
    {
        if (utc.Kind == DateTimeKind.Unspecified) utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        if (utc.Kind == DateTimeKind.Local) utc = utc.ToUniversalTime();

        TimeZoneInfo istZone;
        try
        {
            istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        }
        catch
        {
            try
            {
                istZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
            }
            catch
            {
                // Fallback: add 5.5 hours
                return utc.AddHours(5.5);
            }
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utc, istZone);
    }
}
