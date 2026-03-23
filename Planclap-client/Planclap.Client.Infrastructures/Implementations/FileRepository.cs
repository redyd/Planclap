using Planclap.Client.Domains.Exception;

namespace Planclap.Client.Infrastructures.Implementations;

public abstract class FileRepository
{
    protected FileRepository(string directory, string fileType)
    {
        if (!System.IO.Directory.Exists(directory))
        {
            throw new CouldNotReachDatasourceException("The directory does not exist.");
        }

        Directory = directory;
        FileType = fileType;
    }

    private string FileType { get; }

    private string Directory { get; }

    protected string GetFilePath(DateTime date)
    {
        var path = Path.Combine(Directory, $"{date:yyyy-MM-dd}.{FileType}");
        return !File.Exists(path) ? throw new PlanningNotFoundException($"{FileType} file is not found. Please provide a correct date.") : path;
    }
}
