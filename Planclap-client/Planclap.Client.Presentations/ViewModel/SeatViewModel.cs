using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.ViewModel;

public class SeatViewModel : ViewModelBase, ISeatViewModel
{
    private ActionTRelayCommand<ISeatViewModel>? _clickCommand;
    private bool _isAvailable;
    private bool _isSelected;

    public SeatViewModel(string id, bool isAvailable)
    {
        Id = id;
        IsAvailable = isAvailable;
    }

    public string Id { get; }

    public bool IsAvailable
    {
        get => _isAvailable;
        set
        {
            SetAndRaise(ref _isAvailable, value);
            ClickCommand?.RaiseCanExecuteChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetAndRaise(ref _isSelected, value);
    }

    public ActionTRelayCommand<ISeatViewModel>? ClickCommand
    {
        get => _clickCommand;
        set => SetAndRaise(ref _clickCommand, value);
    }

    public override string ToString() => $"{Id} - {IsAvailable}";
}
