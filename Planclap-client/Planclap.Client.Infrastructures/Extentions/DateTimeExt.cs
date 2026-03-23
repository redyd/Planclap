namespace Planclap.Client.Infrastructures.Extentions;

public static class DateTimeExt
{
    public static long ToTimeStamp(this DateTime date)
        => (long)(date.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;

    public static DateTime ToDateTime(this long timeStamp)
        => DateTime.UnixEpoch.AddSeconds(timeStamp);
}
