namespace Planclap.Client.Domains.Services;

public class TimeService : ITimeService
{
    private readonly DateTime _dateTime;

    public TimeService(DateTime dateTime)
    {
        _dateTime = dateTime;
    }

    public TimeService(DateOnly dateTime)
    {
        _dateTime = dateTime.ToDateTime(TimeStartOfDay);
    }

    public TimeOnly TimeStartOfDay { get; } = new(12, 00);

    public TimeOnly TimeEndOfDay { get; } = new(22, 00);

    public DateTime StartOfDay => _dateTime.Date + TimeStartOfDay.ToTimeSpan();

    public DateTime EndOfDay => _dateTime.Date + TimeEndOfDay.ToTimeSpan();

    public DateOnly CurrentDate => DateOnly.FromDateTime(_dateTime);

    public DateTime Current => _dateTime;

    public DateTime StartOfTheWeek
    {
        get
        {
            var diff = (7 + (_dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            var monday = DateOnly.FromDateTime(_dateTime.AddDays(-diff));
            return monday.ToDateTime(TimeStartOfDay);
        }
    }

    public DateTime EndOfTheWeek
    {
        get
        {
            var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)_dateTime.DayOfWeek + 7) % 7;

            var sunday = DateOnly.FromDateTime(_dateTime.AddDays(daysUntilSunday));
            return sunday.ToDateTime(TimeEndOfDay);
        }
    }

    public DateTime ToStartOfDay(DateOnly time) => time.ToDateTime(TimeStartOfDay);

    public DateTime ToEndOfDay(DateOnly time) => time.ToDateTime(TimeEndOfDay);
}
