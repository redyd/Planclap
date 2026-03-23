using Planclap.Client.Infrastructures.Dto;

namespace Planclap.Client.Infrastructures.IHelpers;

public interface ICsvReaderHelper
{
    IList<CsvScheduledDto> ReadSchedules(string filePath);
}
