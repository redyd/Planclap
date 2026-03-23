using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Domains.core;

public record MovieSession(IScheduled Scheduled, Movie Movie)
{
    public DateTime DateForSession => Scheduled.StartTime;

    public bool IsAvailable(DateTime current) => Scheduled.StartTime >= current;
}
