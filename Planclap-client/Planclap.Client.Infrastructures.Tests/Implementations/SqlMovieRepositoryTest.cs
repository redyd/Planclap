using System.Data.Common;
using Microsoft.Data.Sqlite;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.Exception;
using Planclap.Client.Domains.Services;
using Planclap.Client.Infrastructures.Dto;
using Planclap.Client.Infrastructures.Helpers;
using Planclap.Client.Infrastructures.IHelpers;
using Planclap.Client.Infrastructures.Implementations;
using Planclap.Client.Infrastructures.Mapper;
using Serilog;

namespace Planclap.Client.Infrastructures.Tests.Implementations;

public class SqlMovieRepositoryTest : FileTestHelper
{
    private string _connectionString = string.Empty;
    private DbProviderFactory _factory = null!;
    private SqlMovieRepository _repository = null!;

    private IMovieQuerySetter _query = null!;
    private ILogger _logger = null!;

    [SetUp]
    public void Setup()
    {
        var tempDirectory = CreateTempDirectory();
        var tempDbPath = CreateTempFileInDirectory("planclap-test.sqlite", tempDirectory, "test.db");

        _connectionString = $"Data Source={tempDbPath}";
        _factory = SqliteFactory.Instance;

        _logger = Substitute.For<ILogger>();

        _query = new QuerySetter(new TimeService(new DateTime(2025, 11, 11)));
        _repository = new SqlMovieRepository(_factory, _connectionString, new MoviesMapper(), _query, _logger);
    }


    [Test]
    public void Should_Get_Every_Movie_For_Given_Date()
    {
        // Arrange
        var slugs = new HashSet<MovieSlug> { new("vaiana-2"), new("dune-deuxieme-partie"), new("kung-fu-panda-4"), new("vice-versa-2") };

        // Act
        var result = _repository.FetchAllBySlug(slugs);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(4), "Should return 4 movies scheduled for 2025-11-11");

        var movieSlugs = result.Select(m => m.Slug.Value).ToList();
        Assert.That(movieSlugs, Does.Contain("vaiana-2"));
        Assert.That(movieSlugs, Does.Contain("dune-deuxieme-partie"));
        Assert.That(movieSlugs, Does.Contain("kung-fu-panda-4"));
        Assert.That(movieSlugs, Does.Contain("vice-versa-2"));
    }

    [Test]
    public void Should_Return_Movies_With_Correct_Cinechecks()
    {
        // Arrange
        var slugs = new HashSet<MovieSlug> { new("dune-deuxieme-partie") };

        // Act
        var result = _repository.FetchAllBySlug(slugs);

        // Assert
        var duneMovie = result.First(m => m.Slug.Value == "dune-deuxieme-partie");
        Assert.That(duneMovie, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(duneMovie.CineChecks.Size, Is.EqualTo(2));
            Assert.That(duneMovie.CineChecks.Age, Is.EqualTo(new CineCheckAge("14")));
            Assert.That(duneMovie.CineChecks.CineChecks, Does.Contain(new CineCheck("VIOLENCE")));
        }
    }

    [Test]
    public void Should_Return_Empty_List_For_Date_Without_Shows()
    {
        // Arrange
        var slugs = new HashSet<MovieSlug> { new("vaiana-2") };

        _repository = new SqlMovieRepository(_factory, _connectionString, new MoviesMapper(), new QuerySetter(new TimeService(new DateTime(2025, 11, 13))));

        // Act
        var result = _repository.FetchAllBySlug(slugs);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Should_Return_Movies_With_Complete_Information()
    {
        // Arrange
        var slugs = new HashSet<MovieSlug> { new("vaiana-2") };

        // Act
        var result = _repository.FetchAllBySlug(slugs);

        // Assert
        var vaiana = result.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vaiana.Slug.Value, Is.EqualTo("vaiana-2"));
            Assert.That(vaiana.Title.Value, Is.EqualTo("Vaiana 2"));
            Assert.That(vaiana.PosterUrl.AbsoluteUri, Is.Not.Null.And.Not.Empty);
            Assert.That(vaiana.Description.Value, Is.Not.Null.And.Not.Empty);
            Assert.That(vaiana.CineChecks.Size, Is.Not.Zero);
        }
    }

    [Test]
    public void Should_Throw_Database_Exception()
    {
        _query = Substitute.For<IQuerySetter>();
        _query.ExecuteFetchBySlugQuery(Arg.Any<ISqlWrapper>(), Arg.Any<HashSet<MovieSlug>>()).Throws(new SqliteException("Something went wrong", -1));
        _repository = new SqlMovieRepository(_factory, _connectionString, new MoviesMapper(), _query);

        Assert.Throws<DatabaseException>(() => _repository.FetchAllBySlug(new HashSet<MovieSlug> { new("test") }));
    }

    [Test]
    public void Should_Call_Logger_On_Successful_Fetch()
    {
        // Arrange
        _query = Substitute.For<IMovieQuerySetter>();
        var mapper = Substitute.For<IMovieMapper>();

        var slugs = new HashSet<MovieSlug> { new("vaiana-2") };
        var wrapper = Substitute.For<ISqlWrapper>();
        var query = Substitute.For<ISqlWrapper>();
        _query.ExecuteFetchBySlugQuery(wrapper, slugs).Returns(query);
        query.ExecuteQuery(Arg.Any<Func<DbDataReader, object>>()).Returns(new List<object>());
        mapper.MapFromDto(Arg.Any<IList<MovieDto>>()).Returns(new List<Movie>());

        // Act
        _repository.FetchAllBySlug(slugs);

        // Assert
        _logger.Received().Information("Starting to fetch movies with {Count} slugs", slugs.Count);
        _logger.Received().Information("Fetched {Count} movies successfully", Arg.Any<int>());
    }

    [Test]
    public void Should_Call_Logger_On_DbException()
    {
        // Arrange
        _query = Substitute.For<IMovieQuerySetter>();
        _repository = new SqlMovieRepository(_factory, _connectionString, new MoviesMapper(), _query, _logger);

        var slugs = new HashSet<MovieSlug> { new("vaiana-2") };
        _query.ExecuteFetchBySlugQuery(Arg.Any<ISqlWrapper>(), slugs)
            .Throws(new SqliteException("Something went wrong", -1));

        // Act & Assert
        Assert.Throws<DatabaseException>(() => _repository.FetchAllBySlug(slugs));
        _logger.Received().Warning("Error fetching movies");
    }

    [Test]
    public void Should_Call_Logger_On_Generic_Exception()
    {
        // Arrange
        _query = Substitute.For<IMovieQuerySetter>();
        _repository = new SqlMovieRepository(_factory, _connectionString, new MoviesMapper(), _query, _logger);
        var slugs = new HashSet<MovieSlug> { new("vaiana-2") };
        _query.ExecuteFetchBySlugQuery(Arg.Any<ISqlWrapper>(), slugs)
            .Throws(new Exception("Generic error"));

        // Act & Assert
        Assert.Throws<Exception>(() => _repository.FetchAllBySlug(slugs));
        _logger.Received().Error(Arg.Any<Exception>(), "Unexpected error fetching movies");
    }
}
