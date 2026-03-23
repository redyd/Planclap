namespace Planclap.Client.Domains.Exception;

public class PlanningNotFoundException : RepositoryException
{
    public PlanningNotFoundException(string message) : base(message)
    {
    }
}
