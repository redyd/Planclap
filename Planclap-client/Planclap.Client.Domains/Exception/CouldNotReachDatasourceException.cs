namespace Planclap.Client.Domains.Exception;

public class CouldNotReachDatasourceException : RepositoryException
{
    public CouldNotReachDatasourceException()
    {
    }

    public CouldNotReachDatasourceException(string message) : base(message)
    {
    }
}
