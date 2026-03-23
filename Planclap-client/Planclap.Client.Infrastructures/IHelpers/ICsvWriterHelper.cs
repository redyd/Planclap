using Planclap.Client.Domains.IEntities;

namespace Planclap.Client.Infrastructures.IHelpers;

public interface ICsvWriterHelper
{
    void UpdateReservation(string filePath, string targetDateTime, IReservation reservation);
}
