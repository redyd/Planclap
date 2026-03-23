namespace Planclap.Client.Domains.Services;

public interface ITimeService
{
    TimeOnly TimeStartOfDay { get; }

    TimeOnly TimeEndOfDay { get; }

    DateTime StartOfDay { get; }

    DateTime EndOfDay { get; }

    DateTime StartOfTheWeek { get; }

    DateTime EndOfTheWeek { get; }

    DateOnly CurrentDate { get; }

    DateTime Current { get; }

    DateTime ToStartOfDay(DateOnly time);

    DateTime ToEndOfDay(DateOnly time);
}
