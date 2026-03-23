namespace Planclap.Client.Domains.Events;

/**
 * <summary>
 * Evenement pour notifier un message sur l'écran principal.
 * Type :
 * 'e' → erreur.
 * 's' → succès.
 * </summary>
 */
public class RepositoryErrorEventArgs(string message, char type) : EventArgs
{
    public string Message { get; } = message;

    public char Type { get; } = type;
}
