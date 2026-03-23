using Planclap.Client.Infrastructures.Helpers;

namespace Planclap.Client.Infrastructures.Tests.Helpers;

[TestFixture]
public class MySqlQueryProviderTests
{
    private MySqlQueryProvider _provider = null!;

    [SetUp]
    public void SetUp() => _provider = new MySqlQueryProvider();

    [Test]
    public void Should_ReturnCorrectLastInsertIdQuery_When_Accessed()
    {
        // Act
        var result = _provider.LastInsertIdQuery;

        // Assert
        Assert.That(result, Is.EqualTo("SELECT CAST(LAST_INSERT_ID() AS SIGNED);"));
    }

    [Test]
    public void Should_ConcatMultipleExpressions_When_CalledWithParams()
    {
        // Act
        var result = _provider.Concat("col1", "col2", "'test'");

        // Assert
        Assert.That(result, Is.EqualTo("CONCAT(col1, col2, 'test')"));
    }

    [Test]
    public void Should_GroupConcatWithoutDistinctOrSeparator_When_DefaultParameters()
    {
        // Act
        var result = _provider.GroupConcat("col");

        // Assert
        Assert.That(result, Is.EqualTo("GROUP_CONCAT(col)"));
    }

    [Test]
    public void Should_GroupConcatWithDistinct_When_DistinctTrue()
    {
        // Act
        var result = _provider.GroupConcat("col", distinct: true);

        // Assert
        Assert.That(result, Is.EqualTo("GROUP_CONCAT(DISTINCT col)"));
    }

    [Test]
    public void Should_GroupConcatWithSeparator_When_SeparatorSpecified()
    {
        // Act
        var result = _provider.GroupConcat("col", separator: ';');

        // Assert
        Assert.That(result, Is.EqualTo("GROUP_CONCAT(col SEPARATOR ';')"));
    }

    [Test]
    public void Should_GroupConcatWithDistinctAndSeparator_When_BothSpecified()
    {
        // Act
        var result = _provider.GroupConcat("col", separator: ',', distinct: true);

        // Assert
        Assert.That(result, Is.EqualTo("GROUP_CONCAT(DISTINCT col SEPARATOR ',')"));
    }

    [Test]
    public void Should_HandleEmptyExpressionsInConcat_When_NoExpressionsProvided()
    {
        // Act
        var result = _provider.Concat();

        // Assert
        Assert.That(result, Is.EqualTo("CONCAT()"));
    }
}
