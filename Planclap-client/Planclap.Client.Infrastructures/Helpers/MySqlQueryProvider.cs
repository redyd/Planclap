using System.Text;
using Planclap.Client.Infrastructures.IHelpers;

namespace Planclap.Client.Infrastructures.Helpers;

public class MySqlQueryProvider : ISqlQueryProvider
{
    public string LastInsertIdQuery
        => "SELECT CAST(LAST_INSERT_ID() AS SIGNED);";

    public string Concat(params string[] expressions)
        => $"CONCAT({string.Join(", ", expressions)})";

    public string GroupConcat(string expression, char? separator = null, bool distinct = false)
    {
        var bd = new StringBuilder();
        bd.Append("GROUP_CONCAT(");
        if (distinct)
        {
            bd.Append("DISTINCT ");
        }

        bd.Append(expression);

        if (separator != null)
        {
            bd.Append($" SEPARATOR '{separator}'");
        }

        bd.Append(')');
        return bd.ToString();
    }
}
