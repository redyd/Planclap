using System.Text;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class SqliteQueryProvider : ISqlQueryProvider
{
    public string LastInsertIdQuery
        => "SELECT last_insert_rowid();";

    public string Concat(params string[] expressions) =>
        string.Join(" || ", expressions);

    public string GroupConcat(string expression, char? separator = null, bool distinct = false)
    {
        if (distinct && separator != null)
        {
            throw new ArgumentException("You cannot add a custom separator with DISTINCT clause in Sqlite");
        }

        var bd = new StringBuilder();
        bd.Append("GROUP_CONCAT(");

        if (distinct)
        {
            bd.Append("DISTINCT ");
        }

        bd.Append(expression);

        if (separator != null)
        {
            bd.Append($", '{separator}'");
        }

        bd.Append(')');
        return bd.ToString();
    }
}
