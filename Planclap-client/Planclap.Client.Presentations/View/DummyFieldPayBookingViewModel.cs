using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummyFieldPayBookingViewModel(
    string inputValue,
    string seatId,
    bool isValid,
    string warningInput,
    double price) : IFieldPayBookingViewModel
{
    public string InputValue { get; set; } = inputValue;

    public string SeatId { get; } = seatId;

    public bool IsValid { get; } = isValid;

    public string WarningInput { get; } = warningInput;

    public double Price { get; } = price;
}
