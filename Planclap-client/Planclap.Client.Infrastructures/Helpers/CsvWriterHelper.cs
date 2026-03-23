using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Exceptions;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class CsvWriterHelper : ICsvWriterHelper
{
    public void UpdateReservation(string filePath, string targetDateTime, IReservation reservation)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

        using (var reader = File.OpenText(filePath))
        using (var writer = File.CreateText(tempPath))
        {
            var headerLine = reader.ReadLine()
                             ?? throw new EmptyFileException("CSV file is empty");

            var headers = CsvHeaderParser.Parse(headerLine);
            var hasReservations = headers.ContainsKey("reservations");

            WriteHeader(writer, headerLine, headers, hasReservations);

            ProcessRows(reader, writer, headers, targetDateTime, reservation, hasReservations);
        }

        File.Copy(tempPath, filePath, true);
        File.Delete(tempPath);
    }

    private static void ProcessRows(
        StreamReader reader,
        StreamWriter writer,
        IDictionary<string, int> headers,
        string targetDateTime,
        IReservation reservation,
        bool hasReservations)
    {
        while (reader.ReadLine() is { } line)
        {
            var columns = line.Split(',').ToList();

            if (line.StartsWith(targetDateTime))
            {
                CsvReservationUpdater.UpdateReservationColumn(
                    columns,
                    headers,
                    reservation,
                    hasReservations);
            }

            writer.WriteLine(string.Join(",", columns));
        }
    }

    private static void WriteHeader(
        TextWriter writer,
        string headerLine,
        IDictionary<string, int> headers,
        bool hasReservations)
    {
        if (!hasReservations)
        {
            headers["reservations"] = headers.Count;
            writer.WriteLine($"{headerLine},reservations");
        }
        else
        {
            writer.WriteLine(headerLine);
        }
    }
}
