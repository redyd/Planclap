using System.Runtime.CompilerServices;

namespace Planclap.Client.Infrastructures.Helpers;

/// <summary>
///     Fournit des méthodes de validation contractuelle pour vérifier les conditions préalables.
/// </summary>
internal static class Contract
{
    /// <summary>
    ///     Vérifie qu'un objet de type référence n'est pas null.
    /// </summary>
    /// <typeparam name="T">Le type de l'objet à vérifier (doit être un type référence).</typeparam>
    /// <param name="obj">L'objet à valider.</param>
    /// <param name="paramName">Le nom du paramètre (capturé automatiquement).</param>
    /// <param name="message">Le message.</param>
    /// <exception cref="ArgumentNullException">Lancée si <paramref name="obj" /> est null.</exception>
    internal static void EnsureNotNull<T>(T? obj, [CallerArgumentExpression(nameof(obj))] string paramName = "", string message = "") where T : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName, message);
        }
    }

    /// <summary>
    ///     Vérifie qu'un objet de type référence est null.
    /// </summary>
    /// <typeparam name="T">Le type de l'objet à vérifier (doit être un type référence).</typeparam>
    /// <param name="obj">L'objet à valider.</param>
    /// <param name="message">Un message d'erreur personnalisé (optionnel).</param>
    /// <param name="paramName">Le nom du paramètre (capturé automatiquement).</param>
    /// <exception cref="ArgumentException">Lancée si <paramref name="obj" /> n'est pas null.</exception>
    internal static void EnsureNull<T>(T? obj, string message = "", [CallerArgumentExpression(nameof(obj))] string paramName = "") where T : class
    {
        if (obj is null)
        {
            return;
        }

        var errorMessage = string.IsNullOrEmpty(message)
            ? $"Le paramètre '{paramName}' doit être null."
            : message;
        throw new ArgumentException(errorMessage, paramName);
    }
}
