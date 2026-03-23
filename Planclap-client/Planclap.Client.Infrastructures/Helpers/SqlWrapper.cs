using System.Data.Common;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

/// <summary>
///     Wrapper pour simplifier l'exécution de requêtes SQL avec ADO.NET.
///     Fournit une interface fluide pour gérer les connexions, transactions, commandes et paramètres.
/// </summary>
/// <remarks>
///     <para>Auteur : Nicolas Hendrikx.</para>
///     <para>Documentation générée par : Claude (Anthropic).</para>
///     <para>
///         Cette classe encapsule la complexité d'ADO.NET en offrant une API fluide pour:
///         - Gérer les connexions de base de données
///         - Exécuter des requêtes en lecture/écriture
///         - Gérer les transactions manuellement ou en auto-commit
///         - Mapper les résultats vers des objets typés
///         - Gérer automatiquement la libération des ressources.
///     </para>
///     <para>
///         Utilisation typique:
///         <code>
/// // Lecture simple
/// using var wrapper = SqlWrapper.WithAutoCommit(factory, connectionString);
/// var users = wrapper
///     .NewRead("SELECT * FROM Users WHERE Age > @age")
///     .WithParam("@age", 18)
///     .ExecuteQuery(reader => new User {
///         Id = reader.GetInt32(0),
///         Name = reader.GetString(1)
///     });
/// 
/// // Écriture avec transaction
/// using var wrapper = SqlWrapper.WithTransaction(factory, connectionString);
/// try
/// {
///     wrapper.NewWrite("INSERT INTO Users (Name) VALUES (@name)")
///         .WithParam("@name", "John")
///         .Execute();
///     wrapper.Commit();
/// }
/// catch
/// {
///     wrapper.Rollback();
///     throw;
/// }
/// </code>
///     </para>
/// </remarks>
public sealed class SqlWrapper : ISqlWrapper
{
    private readonly DbConnection _connection;
    private DbCommand? _command;
    private DbTransaction? _transaction;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SqlWrapper" /> class.
    /// </summary>
    /// <param name="factory">Factory pour créer les objets ADO.NET spécifiques au provider.</param>
    /// <param name="connectionString">Chaîne de connexion à la base de données.</param>
    /// <param name="useTransaction">Si true, démarre automatiquement une transaction.</param>
    /// <exception cref="NullReferenceException">Si la factory ne peut pas créer de connexion.</exception>
    private SqlWrapper(DbProviderFactory factory, string connectionString, bool useTransaction = false)
    {
        _connection = factory.CreateConnection()
                      ?? throw new NullReferenceException();
        _connection.ConnectionString = connectionString;
        _connection.Open();
        if (useTransaction)
        {
            _transaction = _connection.BeginTransaction();
        }

        QueryProvider = SqlQueryFactory.CreateSqlProvider(_connection);
    }

    public ISqlQueryProvider QueryProvider { get; }

    public ISqlWrapper NewRead(string sql)
    {
        Contract.EnsureNull(_command, "command", "Command already created");

        _command?.Dispose();
        _command = _connection.CreateCommand();
        _command.CommandText = sql;

        if (_transaction != null)
        {
            _command.Transaction = _transaction;
        }

        return this;
    }

    public ISqlWrapper NewWrite(string sql, bool selectLastInsertId = false)
    {
        Contract.EnsureNull(_command, "command", "Command already created");

        _command?.Dispose();
        _command = _connection.CreateCommand();
        _command.CommandText = sql + ";" + (selectLastInsertId
            ? QueryProvider.LastInsertIdQuery
            : string.Empty);

        if (_transaction != null)
        {
            _command.Transaction = _transaction;
        }

        return this;
    }

    public ISqlWrapper WithParam(string name, object? value)
    {
        Contract.EnsureNotNull(_command, "command", "You must create a command before using this method");

        var parameter = _command!.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        _command.Parameters.Add(parameter);

        return this;
    }

    public IReadOnlyList<T> ExecuteQuery<T>(Func<DbDataReader, T> mapper)
    {
        Contract.EnsureNotNull(_command, "command", "You must create a command before using this method");

        try
        {
            using var reader = _command!.ExecuteReader();
            var list = new List<T>();
            while (reader.Read())
            {
                list.Add(mapper(reader));
            }

            return list;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Error while executing select", e);
        }
        finally
        {
            CloseCommand();
        }
    }

    public T? ExecuteOneQuery<T>(Func<DbDataReader, T> mapper)
    {
        Contract.EnsureNotNull(_command, "command", "You must create a command before using this method");

        try
        {
            using var reader = _command?.ExecuteReader();
            return reader != null && reader.Read() ? mapper(reader) : default;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Error while executing select", e);
        }
        finally
        {
            CloseCommand();
        }
    }

    public int Execute()
    {
        Contract.EnsureNotNull(_command, "command", "You must create a command before using this method");

        try
        {
            return _command!.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Error while executing update", e);
        }
        finally
        {
            CloseCommand();
        }
    }

    public void Commit()
    {
        Contract.EnsureNotNull(_transaction, "transaction", "No transaction started");
        try
        {
            _transaction?.Commit();
            _transaction!.Dispose();
            _transaction = null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error while commiting", ex);
        }
    }

    public void Rollback()
    {
        Contract.EnsureNotNull(_transaction, "transaction", "No transaction started");
        try
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error on rollback", ex);
        }
    }

    /// <summary>
    ///     Libère toutes les ressources utilisées par le wrapper.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Cette méthode effectue automatiquement un rollback si une transaction est active
    ///         et n'a pas été explicitement committée.
    ///     </para>
    ///     <para>
    ///         Ferme également la commande courante (si elle existe) et la connexion à la base de données.
    ///     </para>
    ///     <para>
    ///         Il est recommandé d'utiliser le pattern using pour garantir l'appel de Dispose :
    ///         <code>
    /// using var wrapper = SqlWrapper.WithAutoCommit(factory, connectionString) ;
    /// // ... utilisation du wrapper
    /// </code>
    ///     </para>
    /// </remarks>
    public void Dispose()
    {
        CloseCommand();
        if (_transaction is not null)
        {
            Rollback();
        }

        _connection.Dispose();
    }

    /// <summary>
    ///     Crée un wrapper avec gestion manuelle de transaction.
    /// </summary>
    /// <param name="factory">Factory pour créer les objets ADO.NET.</param>
    /// <param name="connectionString">Chaîne de connexion.</param>
    /// <returns>Une nouvelle instance de <see cref="SqlWrapper" /> avec transaction active.</returns>
    /// <remarks>
    ///     Avec ce mode, vous devez appeler explicitement <see cref="Commit" /> ou <see cref="Rollback" />.
    ///     Si le wrapper est disposé sans commit, un rollback automatique est effectué.
    /// </remarks>
    public static ISqlWrapper WithTransaction(DbProviderFactory factory, string connectionString)
        => new SqlWrapper(factory, connectionString, true);

    /// <summary>
    ///     Crée un wrapper en mode auto-commit (sans transaction explicite).
    /// </summary>
    /// <param name="factory">Factory pour créer les objets ADO.NET.</param>
    /// <param name="connectionString">Chaîne de connexion.</param>
    /// <returns>Une nouvelle instance de <see cref="SqlWrapper" /> sans transaction.</returns>
    /// <remarks>
    ///     Chaque commande est automatiquement committée par le provider de base de données.
    ///     Utilisez ce mode pour des opérations simples qui ne nécessitent pas d'atomicité.
    /// </remarks>
    public static ISqlWrapper WithAutoCommit(DbProviderFactory factory, string connectionString)
        => new SqlWrapper(factory, connectionString);

    /// <summary>
    ///     Ferme et dispose la commande courante.
    /// </summary>
    /// <remarks>
    ///     Méthode privée appelée automatiquement après chaque exécution.
    ///     Réinitialise _command à null pour permettre la création d'une nouvelle commande.
    /// </remarks>
    private void CloseCommand()
    {
        _command?.Dispose();
        _command = null;
    }
}
