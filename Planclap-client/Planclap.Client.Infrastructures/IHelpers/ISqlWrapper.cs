using System.Data.Common;

namespace Planclap.Client.Infrastructures.IHelpers;

public interface ISqlWrapper : IDisposable
{
    ISqlQueryProvider QueryProvider { get; }

    /// <summary>
    ///     Prépare une commande de lecture (SELECT).
    /// </summary>
    /// <param name="sql">Requête SQL SELECT à exécuter.</param>
    /// <returns>L'instance courante pour chaînage fluide.</returns>
    /// <exception cref="InvalidOperationException">Si une commande existe déjà (via <see cref="Contract.EnsureNull" />).</exception>
    /// <remarks>
    ///     Cette méthode doit être appelée avant d'ajouter des paramètres ou d'exécuter la requête.
    ///     Une seule commande peut être active à la fois.
    /// </remarks>
    ISqlWrapper NewRead(string sql);

    /// <summary>
    ///     Prépare une commande d'écriture (INSERT, UPDATE, DELETE).
    /// </summary>
    /// <param name="sql">Requête SQL d'écriture à exécuter.</param>
    /// <param name="selectLastInsertId">
    ///     Si true, ajoute automatiquement la requête pour récupérer le dernier ID inséré.
    /// </param>
    /// <returns>L'instance courante pour chaînage fluide.</returns>
    /// <exception cref="InvalidOperationException">Si une commande existe déjà.</exception>
    /// <remarks>
    ///     Lorsque <paramref name="selectLastInsertId" /> est true, la requête <see cref="LastInsertIdQuery" />
    ///     est automatiquement ajoutée pour permettre la récupération de l'ID via <see cref="Execute{T}(Func{object, T})" />.
    /// </remarks>
    ISqlWrapper NewWrite(string sql, bool selectLastInsertId = false);

    /// <summary>
    ///     Ajoute un paramètre à la commande courante.
    /// </summary>
    /// <param name="name">Nom du paramètre (avec ou sans préfixe selon le provider).</param>
    /// <param name="value">Valeur du paramètre. Peut être null.</param>
    /// <returns>L'instance courante pour chaînage fluide.</returns>
    /// <exception cref="InvalidOperationException">Si aucune commande n'a été créée.</exception>
    /// <remarks>
    ///     Cette méthode peut être appelée plusieurs fois pour ajouter plusieurs paramètres.
    ///     Les paramètres protègent contre les injections SQL.
    ///     Exemple: WithParam("@name", "John").WithParam("@age", 25).
    /// </remarks>
    ISqlWrapper WithParam(string name, object? value);

    /// <summary>
    ///     Exécute une requête SELECT et retourne une liste d'objets mappés.
    /// </summary>
    /// <typeparam name="T">Type des objets retournés.</typeparam>
    /// <param name="mapper">Fonction pour convertir chaque ligne (DbDataReader) en objet T.</param>
    /// <returns>Liste en lecture seule des objets mappés.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Si aucune commande n'existe ou si une erreur survient lors de l'exécution.
    /// </exception>
    /// <remarks>
    ///     La commande est automatiquement fermée après l'exécution (succès ou échec).
    ///     Le mapper est appelé pour chaque ligne retournée par la requête.
    /// </remarks>
    /// <example>
    ///     <code>
    /// var users = wrapper
    ///     .NewRead("SELECT Id, Name FROM Users")
    ///     .ExecuteQuery(r => new User {
    ///         Id = r.GetInt32(0),
    ///         Name = r.GetString(1)
    ///     });
    /// </code>
    /// </example>
    IReadOnlyList<T> ExecuteQuery<T>(Func<DbDataReader, T> mapper);

    /// <summary>
    ///     Exécute une requête SELECT et retourne un seul objet mappé (ou null si aucun résultat).
    /// </summary>
    /// <typeparam name="T">Type de l'objet retourné (doit être un type référence).</typeparam>
    /// <param name="mapper">Fonction pour convertir la première ligne en objet T.</param>
    /// <returns>L'objet mappé ou null si aucune ligne n'est retournée.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Si aucune commande n'existe ou si une erreur survient lors de l'exécution.
    /// </exception>
    /// <remarks>
    ///     Seule la première ligne est lue, même si la requête retourne plusieurs lignes.
    ///     La commande est automatiquement fermée après l'exécution.
    /// </remarks>
    /// <example>
    ///     <code>
    /// var user = wrapper
    ///     .NewRead("SELECT Id, Name FROM Users WHERE Id = @id")
    ///     .WithParam("@id", 1)
    ///     .ExecuteOneQuery(r => new User {
    ///         Id = r.GetInt32(0),
    ///         Name = r.GetString(1)
    ///     });
    /// </code>
    /// </example>
    T? ExecuteOneQuery<T>(Func<DbDataReader, T> mapper);

    /// <summary>
    ///     Exécute une commande d'écriture sans récupération de valeur.
    /// </summary>
    /// <returns>Le nombre de lignes affectées.</returns>
    /// <exception cref="InvalidOperationException">Si aucune commande n'existe ou si une erreur survient.</exception>
    /// <remarks>
    ///     Utilisé pour les INSERT, UPDATE, DELETE qui ne nécessitent pas de récupérer un ID.
    ///     La commande est automatiquement fermée après l'exécution.
    /// </remarks>
    /// <example>
    ///     <code>
    /// int rowsAffected = wrapper
    ///     .NewWrite("UPDATE Users SET Name = @name WHERE Id = @id")
    ///     .WithParam("@name", "Jane")
    ///     .WithParam("@id", 1)
    ///     .Execute();
    /// </code>
    /// </example>
    int Execute();

    /// <summary>
    ///     Valide (commit) la transaction en cours.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Si aucune transaction n'est active (wrapper créé avec WithAutoCommit).
    /// </exception>
    /// <remarks>
    ///     Ne doit être appelé que si le wrapper a été créé avec <see cref="WithTransaction" />.
    ///     Après un commit, la transaction reste techniquement ouverte mais toutes les opérations sont validées.
    /// </remarks>
    void Commit();

    /// <summary>
    ///     Annule (rollback) la transaction en cours.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Si aucune transaction n'est active (wrapper créé avec WithAutoCommit).
    /// </exception>
    /// <remarks>
    ///     Ne doit être appelé que si le wrapper a été créé avec <see cref="WithTransaction" />.
    ///     Annule toutes les modifications effectuées depuis le début de la transaction.
    /// </remarks>
    void Rollback();
}
