using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Exceptions;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class CsvReaderHelper : ICsvReaderHelper
{
    public IList<CsvScheduledDto> ReadSchedules(string filePath)
    {
        using var reader = File.OpenText(filePath);

        var headerLine = reader.ReadLine()
                         ?? throw new EmptyFileException("CSV file is empty");

        var headers = CsvHeaderParser.Parse(headerLine);
        var result = new List<CsvScheduledDto>();

        while (reader.ReadLine() is { } line)
        {
            result.Add(ParseScheduledRow(line, headers));
        }

        return result;
    }

    private static CsvScheduledDto ParseScheduledRow(string line, IDictionary<string, int> headers)
    {
        var columns = line.Split(',').ToList();

        ValidateRequiredColumns(headers);

        var plannedDate = GetColumn(columns, headers, "date");
        var startTime = GetColumn(columns, headers, "startTime");
        var endTime = GetColumn(columns, headers, "endTime");
        var slug = GetColumn(columns, headers, "slug");
        var reservations = GetOptionalColumn(columns, headers, "reservations");

        return new CsvScheduledDto(plannedDate, startTime, endTime, slug, reservations);
    }

    private static void ValidateRequiredColumns(IDictionary<string, int> headers)
    {
        string[] required = ["date", "startTime", "endTime", "slug"];

        foreach (var column in required)
        {
            if (!headers.ContainsKey(column))
            {
                throw new InvalidCsvLineException($"Missing required column: {column}");
            }
        }
    }

    private static string GetColumn(List<string> columns, IDictionary<string, int> headers, string columnName)
    {
        if (!headers.TryGetValue(columnName, out var index))
        {
            throw new InvalidCsvLineException($"Column '{columnName}' not found in headers");
        }

        return index >= columns.Count
            ? throw new InvalidCsvLineException($"Column '{columnName}' missing in row")
            : columns[index];
    }

    private static string GetOptionalColumn(List<string> columns, IDictionary<string, int> headers, string columnName)
    {
        if (!headers.TryGetValue(columnName, out var index) || index >= columns.Count)
        {
            return string.Empty;
        }

        return columns[index];
    }
}
