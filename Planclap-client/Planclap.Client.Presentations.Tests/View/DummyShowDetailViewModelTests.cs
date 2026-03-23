using Planclap.Client.Presentations.View;

namespace Planclap.Client.Presentations.Tests.View;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class DummyShowDetailViewModelTests
{
    [Test]
    public void Tags_ShouldContainExpectedImagePaths()
    {
        // Arrange
        var vm = new DummyShowDetailViewModel();

        var basePath = Path.Combine("Resources", "Images");

        var expected = new[] { Path.Combine(basePath, "12.png"), Path.Combine(basePath, "discrimination.png"), Path.Combine(basePath, "sex.png") };

        // Act
        var actual = vm.Tags;

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual, Has.Count.EqualTo(expected.Length));
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Poster_ShouldReturnExpectedUrl()
    {
        var vm = new DummyShowDetailViewModel();

        Assert.That(vm.Poster, Is.EqualTo("https://theposterdb.com/api/assets/468365/view"));
    }

    [Test]
    public void Title_ShouldReturnExpectedText()
    {
        var vm = new DummyShowDetailViewModel();

        Assert.That(vm.Title, Is.EqualTo("Dummy Show Detail"));
    }

    [Test]
    public void Duration_ShouldReturn120Minutes()
    {
        var vm = new DummyShowDetailViewModel();

        Assert.That(vm.Duration, Is.EqualTo(TimeSpan.FromMinutes(120)));
    }

    [Test]
    public void Description_ShouldNotBeNullOrEmpty()
    {
        var vm = new DummyShowDetailViewModel();

        Assert.That(vm.Description, Is.Not.Null);
        Assert.That(vm.Description, Is.Not.Empty);
    }
}
