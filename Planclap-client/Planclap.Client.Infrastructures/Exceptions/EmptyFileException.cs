using Planclap.Client.Domains.Exception;

namespace Planclap.Client.Infrastructures.Exceptions;

public class EmptyFileException : RepositoryException
{
    public EmptyFileException()
    {
    }

    public EmptyFileException(string message) : base(message)
    {
    }

    public EmptyFileException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
