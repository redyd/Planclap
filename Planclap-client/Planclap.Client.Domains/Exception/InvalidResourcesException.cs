namespace Planclap.Client.Domains.Exception;

public class InvalidResourcesException : RepositoryException
{
    public InvalidResourcesException(string message) : base(message)
    {
    }
}
