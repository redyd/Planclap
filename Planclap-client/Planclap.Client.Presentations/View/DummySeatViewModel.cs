using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummySeatViewModel : ViewModelBase, ISeatViewModel
{
    private bool _isAvailable;
    private bool _isSelected;

    public DummySeatViewModel(string id = "0 - 0", bool isAvailable = true, bool isSelected = false)
    {
        Id = id;
        _isAvailable = isAvailable;
        _isSelected = isSelected;

        ClickCommand = new ActionTRelayCommand<ISeatViewModel>(_ =>
        {
            if (IsAvailable)
            {
                IsSelected = !IsSelected;
            }
        });
    }

    public string Id { get; }

    public bool IsAvailable
    {
        get => _isAvailable;
        set => SetAndRaise(ref _isAvailable, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetAndRaise(ref _isSelected, value);
    }

    public ActionTRelayCommand<ISeatViewModel>? ClickCommand { get; set; }
}
