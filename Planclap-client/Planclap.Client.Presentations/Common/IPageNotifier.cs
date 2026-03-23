namespace Planclap.Client.Presentations.Common;

public interface IPageNotifier
{
    void Subscribe<TEventArgs>(Action<TEventArgs> handler) where TEventArgs : EventArgs;

    void Publish<TEventArgs>(TEventArgs message) where TEventArgs : EventArgs;
}
