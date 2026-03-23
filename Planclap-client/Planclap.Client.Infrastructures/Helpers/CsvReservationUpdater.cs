using Planclap.Client.Domains.IEntities;
using Planclap.Client.Infrastructures.Mapper;

namespace Planclap.Client.Infrastructures.Helpers;

public static class CsvReservationUpdater
{
    public static void UpdateReservationColumn(
        IList<string> columns,
        IDictionary<string, int> headers,
        IReservation reservation,
        bool hasReservations)
    {
        var reservationIndex = headers["reservations"];
        var currentReservations = GetCurrentReservations(columns, reservationIndex, hasReservations);
        var updatedReservations = ReservationMapper.MapNewReservation(reservation, currentReservations);

        EnsureColumnCapacity(columns, reservationIndex);
        columns[reservationIndex] = updatedReservations;
    }

    private static string GetCurrentReservations(IList<string> columns, int reservationIndex, bool hasReservations)
    {
        if (!hasReservations || reservationIndex >= columns.Count)
        {
            return string.Empty;
        }

        return columns[reservationIndex];
    }

    private static void EnsureColumnCapacity(IList<string> columns, int requiredIndex)
    {
        while (columns.Count <= requiredIndex)
        {
            columns.Add(string.Empty);
        }
    }
}
