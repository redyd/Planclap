using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Implementations;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Tests.Implementations;

[TestFixture]
public class JsonMovieFileRepositoryTest : FileTestHelper
{
    [SetUp]
    public void Setup()
    {
        TempDirectoryBasic = CreateTempDirectory();
        TempFileBasic = CreateTempFileInDirectory("movies.json", TempDirectoryBasic, "2024-12-30.json");

        TempDirectoryWrong = CreateTempDirectory();
        CreateTempFileInDirectory("movies-wrong.json", TempDirectoryWrong, "2024-12-30.json");

        _date = new DateOnly(2025, 01, 01);
        _time = new TimeService(_date);
    }

    private string TempDirectoryBasic { get; set; } = string.Empty;
    private string TempDirectoryWrong { get; set; } = string.Empty;
    private string TempFileBasic { get; set; } = string.Empty;

    private DateOnly _date;

    private ITimeService _time = null!;

    [Test]
    public void Should_Fetch_Every_Movie()
    {
        var repo = new JsonMovieFileRepository(TempDirectoryBasic, new MoviesMapper(), _time, new LoggerConfiguration().CreateLogger());
        var slugs = new HashSet<MovieSlug> { new("godzilla-x-kong-le-nouvel-empire"), new("dune-deuxieme-partie"), new("vice-versa-2"), new("vaiana-2") };
        var results = repo.FetchAllBySlug(slugs);

        Assert.That(results, Is.Not.Null);
        Assert.That(results, Has.Count.EqualTo(4));
    }

    [Test]
    public void Should_Throws_Exception_When_Invalid_Date()
    {
        var repo = new JsonMovieFileRepository(TempDirectoryBasic, new MoviesMapper(), new TimeService(new DateOnly(1990, 12, 12)));
        var slugs = new HashSet<MovieSlug> { new("godzilla-x-kong-le-nouvel-empire"), new("dune-deuxieme-partie"), new("vice-versa-2"), new("vaiana-2") };

        Assert.That(() => repo.FetchAllBySlug(slugs), Throws.TypeOf<PlanningNotFoundException>());
    }

    [Test]
    public void Should_Throws_Exception_When_Empty_File()
    {
        var repo = new JsonMovieFileRepository(TempDirectoryBasic, new MoviesMapper(), _time);

        // clear file
        File.WriteAllText(TempFileBasic, string.Empty);

        var slugs = new HashSet<MovieSlug> { new("godzilla-x-kong-le-nouvel-empire"), new("dune-deuxieme-partie"), new("vice-versa-2"), new("vaiana-2") };

        Assert.That(() => repo.FetchAllBySlug(slugs), Throws.TypeOf<InvalidResourcesException>());
    }
}
