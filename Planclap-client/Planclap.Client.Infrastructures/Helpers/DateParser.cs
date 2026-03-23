using System.Globalization;

namespace Planclap.Client.Infrastructures.Helpers;

public class DateParser
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimeFormat = "HH:mm";

    public static DateTime ParseDateTime(string date, string time)
    {
        var dateOnly = DateOnly.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        var timeOnly = TimeOnly.ParseExact(time, TimeFormat, CultureInfo.InvariantCulture);

        return dateOnly.ToDateTime(timeOnly);
    }

    public static TimeSpan CalculateDuration(string startTime, string endTime)
    {
        var start = TimeOnly.ParseExact(startTime, TimeFormat, CultureInfo.InvariantCulture);
        var end = TimeOnly.ParseExact(endTime, TimeFormat, CultureInfo.InvariantCulture);

        return end - start;
    }
}
