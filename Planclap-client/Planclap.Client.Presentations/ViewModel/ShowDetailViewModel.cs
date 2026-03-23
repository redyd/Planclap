using System.Collections.ObjectModel;
using Planclap.Client.Domains.IEntities;
using Planclap.Client.Presentations.Common;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.IViewModel;
using Planclap.Client.Presentations.Mapper;
using Planclap.Client.Presentations.Routers;

namespace Planclap.Client.Presentations.ViewModel;

public class ShowDetailViewModel : ViewModelBase, IShowDetailViewModel
{
    // data
    private readonly IShowDetailDtoMapper _mapper;

    // object
    private readonly IPlanning _planning;
    private readonly ShowDetailSubInfo _subInfo;
    private readonly ObservableCollection<string> _tags = new();

    public ShowDetailViewModel(IPageNotifier pageNotifier, IPlanning planning, IShowDetailDtoMapper mapper)
    {
        _planning = planning;
        _subInfo = new ShowDetailSubInfo();
        _mapper = mapper;

        pageNotifier.Subscribe<OnMovieClickEvent>(OnMovieClick);
    }

    private ShowDetailDto Infos
    {
        set
        {
            Description = value.Description;
            Poster = value.Poster;
            Duration = value.Duration;
            Title = value.Title;
            Tags = value.Tags;
        }
    }

    public IReadOnlyList<string> Tags
    {
        get => _tags;
        private set
        {
            _tags.Clear();
            foreach (var tag in value)
            {
                _tags.Add(tag);
            }
        }
    }

    public string Title
    {
        get => _subInfo.Title;
        private set
        {
            _subInfo.Title = value;
            NotifyPropertyChanged(nameof(Title));
        }
    }

    public string Description
    {
        get => _subInfo.Description;
        private set
        {
            _subInfo.Description = value;
            NotifyPropertyChanged(nameof(Description));
        }
    }

    public string Poster
    {
        get => _subInfo.Poster;
        private set
        {
            _subInfo.Poster = value;
            NotifyPropertyChanged(nameof(Poster));
        }
    }

    public TimeSpan Duration
    {
        get => _subInfo.Duration;
        private set
        {
            _subInfo.Duration = value;
            NotifyPropertyChanged(nameof(Duration));
        }
    }

    private void OnMovieClick(OnMovieClickEvent e)
    {
        if (e.Slug == null)
        {
            return;
        }

        Infos = _mapper.MapToDto(_planning[e.Slug]);
    }

    private class ShowDetailSubInfo(string title = "", string description = "", string poster = "", TimeSpan duration = default)
    {
        public string Title { get; set; } = title;

        public string Description { get; set; } = description;

        public string Poster { get; set; } = poster;

        public TimeSpan Duration { get; set; } = duration;
    }
}
