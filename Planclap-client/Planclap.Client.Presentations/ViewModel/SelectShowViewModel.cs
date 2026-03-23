using System.Collections.ObjectModel;
using Planclap.Client.Domains.core;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.PresentationServices;

namespace Planclap.Client.Presentations.ViewModel;

public class SelectShowViewModel : ViewModelBase, ISelectShowViewModel
{
    // object
    private readonly ISelectShowService _service;
    private ShowOverviewDto? _selectedShow;

    // data
    private ObservableCollection<ShowOverviewDto> _shows = new();

    public SelectShowViewModel(ISelectShowService service, IPlanning planning)
    {
        _service = service;
        Shows = MapToDto(planning.Movies);
    }

    // == PROPERTY ==
    public string Title => $"Les séances du {_service.Current:dd MMMM yyyy}";

    public ObservableCollection<ShowOverviewDto> Shows
    {
        get => _shows;
        set => SetAndRaise(ref _shows, value);
    }

    public ShowOverviewDto? SelectedShow
    {
        get => _selectedShow;
        set
        {
            if (SetAndRaise(ref _selectedShow, value) && value != null)
            {
                _service.OnMovieClick(value);
            }
        }
    }

    // == INIT ==
    public void Initialize() => SelectedShow = FirstAvailableShow();

    private ShowOverviewDto? FirstAvailableShow()
    {
        if (Shows.Count == 0)
        {
            return null;
        }

        foreach (var show in Shows)
        {
            if (show.CanBuy)
            {
                return show;
            }
        }

        return Shows.Last();
    }

    // == MAPPING ==
    private ObservableCollection<ShowOverviewDto> MapToDto(IReadOnlyList<MovieSession> movies)
        => new(movies.Select(movie =>
            new ShowOverviewDto(
                movie.Movie.PosterUrl,
                movie.Movie.Title.Value,
                TimeOnly.FromDateTime(movie.DateForSession),
                movie.Scheduled.Duration,
                movie.IsAvailable(_service.Current))));
}
