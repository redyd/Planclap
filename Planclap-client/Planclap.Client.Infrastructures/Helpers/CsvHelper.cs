using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Exceptions;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class CsvHelper(ICsvReaderHelper csvReader, ICsvWriterHelper csvWriter) : ICsvHelper
{
    public CsvHelper() : this(new CsvReaderHelper(), new CsvWriterHelper())
    {
    }

    public IList<CsvScheduledDto> ReadSchedules(string filePath)
    {
        try
        {
            return csvReader.ReadSchedules(filePath);
        }
        catch (EmptyFileException)
        {
            throw new InvalidResourcesException("File is empty");
        }
        catch (InvalidResourcesException)
        {
            throw new InvalidResourcesException("File is invalid");
        }
    }

    public void UpdateReservation(string filePath, string targetDateTime, IReservation reservation)
    {
        try
        {
            csvWriter.UpdateReservation(filePath, targetDateTime, reservation);
        }
        catch (EmptyFileException)
        {
            throw new InvalidResourcesException("File is empty");
        }
    }
}
