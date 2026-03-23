namespace Planclap.Client.Infrastructures.Tests;

public class FileTestHelper
{
    protected static string CreateTempFileInDirectory(string originFileName, string targetDirectory, string fileName)
    {
        var cwd = Directory.GetCurrentDirectory();
        var originFullPath = Path.Combine(cwd, "..", "..", "..", "Resources", originFileName);
        var tempFile = Path.Combine(targetDirectory, fileName);

        File.Copy(originFullPath, tempFile);

        return tempFile;
    }

    protected static string CreateTempDirectory() => Directory.CreateTempSubdirectory().FullName;
}
