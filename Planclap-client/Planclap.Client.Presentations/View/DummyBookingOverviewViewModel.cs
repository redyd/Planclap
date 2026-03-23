using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummyBookingOverviewViewModel : IBookingOverviewViewModel
{
    private readonly Random _random = new();
    private IReadOnlyList<ISeatViewModel>? _seats;

    public string ReservationTitle => "Film dummy";

    public int Rows => 10;

    public int Columns => 8;

    public IReadOnlyList<ISeatViewModel> Seats
    {
        get
        {
            if (_seats != null)
            {
                return _seats;
            }

            var list = new List<ISeatViewModel>();

            for (var r = 1; r <= Rows; r++)
            {
                for (var c = 1; c <= Columns; c++)
                {
                    var av = _random.NextDouble() > 0.2;
                    var sel = av && _random.NextDouble() < 0.2;

                    list.Add(new DummySeatViewModel(
                        $"{r} - {c}",
                        av,
                        sel));
                }
            }

            _seats = list.AsReadOnly();
            return _seats;
        }
    }

    public ActionRelayCommand OnCreateBookingRequested => new(() => { }, () => true);
}
