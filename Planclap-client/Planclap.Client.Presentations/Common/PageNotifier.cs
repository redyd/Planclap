namespace Planclap.Client.Presentations.Common;

public class PageNotifier : IPageNotifier
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<TEventArgs>(Action<TEventArgs> handler) where TEventArgs : EventArgs
    {
        if (!_handlers.ContainsKey(typeof(TEventArgs)))
        {
            _handlers[typeof(TEventArgs)] = [];
        }

        _handlers[typeof(TEventArgs)].Add(handler);
    }

    public void Publish<TEventArgs>(TEventArgs message) where TEventArgs : EventArgs
    {
        if (!_handlers.TryGetValue(typeof(TEventArgs), out var handlers))
        {
            return;
        }

        foreach (var h in handlers)
        {
            ((Action<TEventArgs>)h)(message);
        }
    }
}
