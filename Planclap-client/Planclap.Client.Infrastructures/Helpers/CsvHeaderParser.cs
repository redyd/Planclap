using Planclap.Client.Domains.Exception;

namespace Planclap.Client.Infrastructures.Helpers;

public static class CsvHeaderParser
{
    public static IDictionary<string, int> Parse(string headerLine)
    {
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            throw new InvalidResourcesException("CSV header is empty");
        }

        return headerLine
            .Split(',')
            .Select((header, index) => (header, index))
            .ToDictionary(x => x.header, x => x.index);
    }
}
