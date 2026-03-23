using System.Windows.Input;

namespace Planclap.Client.Presentations.Common;

public class ActionRelayCommand(Action execute, Func<bool>? canExecute = null)
    : ICommand
{
    private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? _)
        => canExecute == null || canExecute();

    public void Execute(object? _)
        => _execute();

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class ActionTRelayCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null)
    : ICommand
{
    private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (canExecute == null)
        {
            return true;
        }

        return parameter is T t && canExecute(t);
    }

    public void Execute(object? parameter)
    {
        if (parameter is T asT)
        {
            _execute(asT);
        }
    }

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
