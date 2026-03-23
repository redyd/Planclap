using System.Data.Common;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class SqlQueryFactory
{
    public static ISqlQueryProvider CreateSqlProvider(DbConnection connection)
        => connection.GetType().Name switch
        {
            "MySqlConnection" => new MySqlQueryProvider(),
            "SqliteConnection" => new SqliteQueryProvider(),
            _ => throw new ArgumentOutOfRangeException(nameof(connection))
        };
}
