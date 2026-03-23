namespace Planclap.Client.Presentations.IViewModel;

public interface IFieldPayBookingViewModel
{
    string InputValue { get; set; }

    string SeatId { get; }

    bool IsValid { get; }

    string WarningInput { get; }

    double Price { get; }
}
