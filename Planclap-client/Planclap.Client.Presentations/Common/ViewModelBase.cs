using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Planclap.Client.Presentations.Common;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetAndRaise<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (field != null && field.Equals(value))
        {
            return false;
        }

        field = value; //<1>
        NotifyPropertyChanged(propertyName);
        return true;
    }

    protected void NotifyPropertyChanged(string propertyName)
    {
        var eventArg = new PropertyChangedEventArgs(propertyName);
        PropertyChanged?.Invoke(this, eventArg);
    }
}
