using NSubstitute;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.Mapper;
using Planclap.Client.Presentations.Routers;
using Planclap.Client.Presentations.ViewModel;

namespace Planclap.Client.Presentations.Tests.ViewModel;

/// <summary>
///     Classe générée par IA
/// </summary>
[TestFixture]
public class ShowDetailViewModelTests
{
    [SetUp]
    public void SetUp()
    {
        _pageNotifier = Substitute.For<IPageNotifier>();
        _planning = Substitute.For<IPlanning>();
        _mapper = Substitute.For<IShowDetailDtoMapper>();

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);
    }

    private IPageNotifier _pageNotifier = null!;
    private IPlanning _planning = null!;
    private IShowDetailDtoMapper _mapper = null!;
    private ShowDetailViewModel _viewModel = null!;

    // === CONSTRUCTOR TESTS ===

    [Test]
    public void Should_SubscribeToPageNotifier_When_Constructed_Given_ValidDependencies() =>
        // Assert
        _pageNotifier.Received(1).Subscribe(
            Arg.Any<Action<OnMovieClickEvent>>()
        );

    [Test]
    public void Should_InitializeWithEmptyTags_When_Constructed_Given_ValidDependencies() =>
        // Assert
        Assert.That(_viewModel.Tags, Is.Empty);

    [Test]
    public void Should_InitializeWithEmptyTitle_When_Constructed_Given_ValidDependencies() =>
        // Assert
        Assert.That(_viewModel.Title, Is.EqualTo(string.Empty));

    [Test]
    public void Should_InitializeWithEmptyDescription_When_Constructed_Given_ValidDependencies() =>
        // Assert
        Assert.That(_viewModel.Description, Is.EqualTo(string.Empty));

    [Test]
    public void Should_InitializeWithEmptyPoster_When_Constructed_Given_ValidDependencies() =>
        // Assert
        Assert.That(_viewModel.Poster, Is.EqualTo(string.Empty));

    [Test]
    public void Should_InitializeWithZeroDuration_When_Constructed_Given_ValidDependencies() =>
        // Assert
        Assert.That(_viewModel.Duration, Is.EqualTo(TimeSpan.Zero));

    // === EVENT HANDLING - OnMovieClickEvent ===

    [Test]
    public void Should_UpdateAllProperties_When_EventReceived_Given_ValidSlugAndDto()
    {
        // Arrange
        var slug = new MovieSlug("test-movie-title");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto(
            "Test Movie Title",
            "A great movie",
            "poster.jpg",
            TimeSpan.FromMinutes(120),
            new List<string> { "al", "peur" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(_viewModel.Title, Is.EqualTo("Test Movie Title"));
            Assert.That(_viewModel.Description, Is.EqualTo("A great movie"));
            Assert.That(_viewModel.Poster, Does.Contain("poster.jpg"));
            Assert.That(_viewModel.Duration, Is.EqualTo(TimeSpan.FromMinutes(120)));
            Assert.That(_viewModel.Tags, Has.Count.EqualTo(2));
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Tags[0], Is.EqualTo("al"));
            Assert.That(_viewModel.Tags[1], Is.EqualTo("peur"));
        }
    }

    [Test]
    public void Should_CallMapper_When_EventReceived_Given_ValidSlug()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto(
            "Test Movie Title",
            "A great movie",
            "poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al", "FEAR" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        _mapper.Received(1).MapToDto(movieSession);
    }

    [Test]
    public void Should_GetMovieFromPlanning_When_EventReceived_Given_ValidSlug()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto(
            "Test Movie Title",
            "A great movie",
            "poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al", "FEAR" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        _ = _planning.Received(1)[slug];
    }

    [Test]
    public void Should_DoNothing_When_EventReceived_Given_NullSlug()
    {
        // Arrange
        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = null });

        // Assert
        _mapper.DidNotReceive().MapToDto(Arg.Any<MovieSession>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Title, Is.EqualTo(string.Empty));
            Assert.That(_viewModel.Description, Is.EqualTo(string.Empty));
            Assert.That(_viewModel.Poster, Is.EqualTo(string.Empty));
            Assert.That(_viewModel.Duration, Is.EqualTo(TimeSpan.Zero));
            Assert.That(_viewModel.Tags, Is.Empty);
        }
    }

    // === TAGS PROPERTY TESTS ===

    [Test]
    public void Should_ReplaceExistingTags_When_NewTagsSet_Given_DifferentTags()
    {
        // Arrange
        var slug1 = new MovieSlug("movie-1");
        var scheduled1 = Substitute.For<IScheduled>();
        var movie1 = MovieSupplier.Supply("movie-1", "Movie 1");
        var movieSession1 = new MovieSession(scheduled1, movie1);

        var dto1 = new ShowDetailDto(
            "Movie 1",
            "Description 1",
            "poster1.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al", "FEAR" }
        );

        var slug2 = new MovieSlug("movie-2");
        var scheduled2 = Substitute.For<IScheduled>();
        var movie2 = MovieSupplier.Supply("movie-2", "Movie 2");
        var movieSession2 = new MovieSession(scheduled2, movie2);

        var dto2 = new ShowDetailDto
        (
            "Movie 2",
            "Description 2",
            "poster2.jpg",
            TimeSpan.FromMinutes(120),
            new List<string> { "6" }
        );

        _planning[slug1].Returns(movieSession1);
        _planning[slug2].Returns(movieSession2);
        _mapper.MapToDto(movieSession1).Returns(dto1);
        _mapper.MapToDto(movieSession2).Returns(dto2);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug1 });
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug2 });

        // Assert
        Assert.That(_viewModel.Tags, Has.Count.EqualTo(1));
        Assert.That(_viewModel.Tags[0], Is.EqualTo("6"));
    }

    [Test]
    public void Should_Decrement_Tags_Count_When_NewUniqueTagSet_Given_PreviouslyHadTags()
    {
        // Arrange
        var slug1 = new MovieSlug("movie-1");
        var scheduled1 = Substitute.For<IScheduled>();
        var movie1 = MovieSupplier.Supply("movie-1", "Movie 1");
        var movieSession1 = new MovieSession(scheduled1, movie1);

        var dto1 = new ShowDetailDto(
            "Movie 1",
            "Description 1",
            "poster1.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al", "FEAR" }
        );

        var slug2 = new MovieSlug("movie-2");
        var scheduled2 = Substitute.For<IScheduled>();
        var movie2 = MovieSupplier.Supply("movie-2", "Movie 2");
        var movieSession2 = new MovieSession(scheduled2, movie2);

        var dto2 = new ShowDetailDto
        (
            "Movie 2",
            "Description 2",
            "poster2.jpg",
            TimeSpan.FromMinutes(120),
            new List<string> { "6" }
        );

        _planning[slug1].Returns(movieSession1);
        _planning[slug2].Returns(movieSession2);
        _mapper.MapToDto(movieSession1).Returns(dto1);
        _mapper.MapToDto(movieSession2).Returns(dto2);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug1 });
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug2 });

        // Assert
        Assert.That(_viewModel.Tags, Has.Count.EqualTo(1));
    }

    [Test]
    public void Should_HandleMultipleTags_When_EventReceived_Given_DtoWithManyTags()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "Test",
            "Test",
            "poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al", "peur", "sex" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        Assert.That(_viewModel.Tags, Has.Count.EqualTo(3));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_viewModel.Tags[0], Is.EqualTo("al"));
            Assert.That(_viewModel.Tags[1], Is.EqualTo("peur"));
            Assert.That(_viewModel.Tags[2], Is.EqualTo("sex"));
        }
    }

    // === PROPERTY CHANGE NOTIFICATION TESTS ===

    [Test]
    public void Should_RaisePropertyChanged_When_TitleUpdated_Given_NewMovieData()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "New Title",
            "Description",
            "poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.Title))
            {
                propertyChanged = true;
            }
        };

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_DescriptionUpdated_Given_NewMovieData()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "Title",
            "New Description",
            "poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.Description))
            {
                propertyChanged = true;
            }
        };

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_PosterUpdated_Given_NewMovieData()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "Title",
            "Description",
            "new-poster.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al" }
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.Poster))
            {
                propertyChanged = true;
            }
        };

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    [Test]
    public void Should_RaisePropertyChanged_When_DurationUpdated_Given_NewMovieData()
    {
        // Arrange
        var slug = new MovieSlug("test-movie");
        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("test-movie-title", "Test Movie Title");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "Title",
            "Description",
            "poster.jpg",
            TimeSpan.FromMinutes(150),
            new List<string>()
        );

        _planning[slug].Returns(movieSession);
        _mapper.MapToDto(movieSession).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        var propertyChanged = false;
        _viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.Duration))
            {
                propertyChanged = true;
            }
        };

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug });

        // Assert
        Assert.That(propertyChanged, Is.True);
    }

    // === MULTIPLE UPDATES TESTS ===

    [Test]
    public void Should_UpdateAllPropertiesMultipleTimes_When_MultipleEventsReceived_Given_DifferentMovies()
    {
        // Arrange
        var slug1 = new MovieSlug("movie-1");
        var scheduled1 = Substitute.For<IScheduled>();
        var movie1 = MovieSupplier.Supply("movie-1", "Movie 1");
        var movieSession1 = new MovieSession(scheduled1, movie1);

        var dto1 = new ShowDetailDto
        (
            "Movie 1",
            "First Description",
            "poster1.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al" }
        );

        var slug2 = new MovieSlug("movie-2");
        var scheduled2 = Substitute.For<IScheduled>();
        var movie2 = MovieSupplier.Supply("movie-2", "Movie 2");
        var movieSession2 = new MovieSession(scheduled2, movie2);

        var dto2 = new ShowDetailDto
        (
            "Movie 2",
            "Second Description",
            "poster2.jpg",
            TimeSpan.FromMinutes(120),
            new List<string> { "6", "FEAR" }
        );

        _planning[slug1].Returns(movieSession1);
        _planning[slug2].Returns(movieSession2);
        _mapper.MapToDto(movieSession1).Returns(dto1);
        _mapper.MapToDto(movieSession2).Returns(dto2);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug1 });
        var firstTitle = _viewModel.Title;

        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug2 });
        var secondTitle = _viewModel.Title;

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(firstTitle, Is.EqualTo("Movie 1"));
            Assert.That(secondTitle, Is.EqualTo("Movie 2"));
            Assert.That(_viewModel.Title, Is.EqualTo("Movie 2"));
            Assert.That(_viewModel.Description, Is.EqualTo("Second Description"));
            Assert.That(_viewModel.Poster, Is.EqualTo("poster2.jpg"));
            Assert.That(_viewModel.Duration, Is.EqualTo(TimeSpan.FromMinutes(120)));
            Assert.That(_viewModel.Tags, Has.Count.EqualTo(2));
        }
    }

    [Test]
    public void Should_CallMapperMultipleTimes_When_MultipleEventsReceived_Given_DifferentSlugs()
    {
        // Arrange
        var slug1 = new MovieSlug("movie-1");
        var slug2 = new MovieSlug("movie-2");
        var slug3 = new MovieSlug("movie-3");

        var scheduled = Substitute.For<IScheduled>();
        var movie = MovieSupplier.Supply("movie-1", "Movie 1");
        var movieSession = new MovieSession(scheduled, movie);

        var dto = new ShowDetailDto
        (
            "Movie 1",
            "Test description",
            "test.jpg",
            TimeSpan.FromMinutes(90),
            new List<string> { "al" }
        );

        _planning[slug1].Returns(movieSession);
        _planning[slug2].Returns(movieSession);
        _planning[slug3].Returns(movieSession);
        _mapper.MapToDto(Arg.Any<MovieSession>()).Returns(dto);

        Action<OnMovieClickEvent>? capturedAction = null;
        _pageNotifier.When(x => x.Subscribe(
                Arg.Any<Action<OnMovieClickEvent>>()))
            .Do(x => capturedAction = x.Arg<Action<OnMovieClickEvent>>());

        _viewModel = new ShowDetailViewModel(_pageNotifier, _planning, _mapper);

        // Act
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug1 });
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug2 });
        capturedAction?.Invoke(new OnMovieClickEvent { Slug = slug3 });

        // Assert
        _mapper.Received(3).MapToDto(Arg.Any<MovieSession>());
    }
}
