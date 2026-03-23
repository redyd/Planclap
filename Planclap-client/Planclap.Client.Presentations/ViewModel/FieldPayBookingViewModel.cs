using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.ViewModel;

public class FieldPayBookingViewModel(ITicket ticket, Action onUpdate) : ViewModelBase, IFieldPayBookingViewModel
{
    // data
    private string _inputValue = string.Empty;
    private bool _isValid;
    private double _price;
    private string _warningInput = string.Empty;

    public string InputValue
    {
        get => _inputValue;
        set
        {
            var trimmed = value.Trim();
            if (SetAndRaise(ref _inputValue, trimmed))
            {
                ValidateAndUpdate();
            }
        }
    }

    public string SeatId => $"{ticket.Seat.Row} - {ticket.Seat.Column}";

    public bool IsValid
    {
        get => _isValid;
        private set => SetAndRaise(ref _isValid, value);
    }

    public string WarningInput
    {
        get => _warningInput;
        private set => SetAndRaise(ref _warningInput, value);
    }

    public double Price
    {
        get => _price;
        private set => SetAndRaise(ref _price, value);
    }

    private void ValidateAndUpdate()
    {
        if (string.IsNullOrWhiteSpace(InputValue))
        {
            IsValid = false;
            WarningInput = "Requis";
            Price = 0;

            ticket.Invalidate();
            onUpdate.Invoke();
            return;
        }

        if (!int.TryParse(InputValue, out var age))
        {
            IsValid = false;
            WarningInput = "Entrée incorrecte";
            Price = 0;

            ticket.Invalidate();
            onUpdate.Invoke();
            return;
        }

        if (age is < 0 or > 120)
        {
            IsValid = false;
            WarningInput = "Âge invalide";
            Price = 0;

            ticket.Invalidate();
            onUpdate.Invoke();
            return;
        }

        ticket.Age = age;
        IsValid = true;
        WarningInput = string.Empty;
        Price = ticket.Value;

        onUpdate.Invoke();
    }
}
