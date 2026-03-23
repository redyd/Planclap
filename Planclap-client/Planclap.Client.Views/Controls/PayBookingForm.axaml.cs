using Avalonia.Controls;
using Avalonia.Input;

namespace Planclap.Client.Views.Controls;

public partial class PayBookingForm : UserControl
{
    public PayBookingForm()
    {
        InitializeComponent();
        RootGrid.PointerPressed += RootGrid_PointerPressed;
    }

    private void RootGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!FocusCatcher.IsFocused)
        {
            FocusCatcher.Focus();
        }
    }
}
