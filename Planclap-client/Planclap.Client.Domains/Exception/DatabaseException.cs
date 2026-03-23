namespace Planclap.Client.Domains.Exception;

public class DatabaseException : RepositoryException
{
    public DatabaseException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
