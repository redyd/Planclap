namespace Planclap.Client.Infrastructures.IHelpers;

public interface ISqlQueryProvider
{
    string LastInsertIdQuery { get; }

    string Concat(params string[] expressions);

    string GroupConcat(string expression, char? separator = null, bool distinct = false);
}
