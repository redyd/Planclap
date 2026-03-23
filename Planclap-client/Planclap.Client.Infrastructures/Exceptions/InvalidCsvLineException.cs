using Planclap.Client.Domains.Exception;

namespace Planclap.Client.Infrastructures.Exceptions;

public class InvalidCsvLineException : RepositoryException
{
    public InvalidCsvLineException()
    {
    }

    public InvalidCsvLineException(string message) : base(message)
    {
    }

    public InvalidCsvLineException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
