using System.Collections.ObjectModel;
using Planclap.Client.Presentations.Dtos;

namespace Planclap.Client.Presentations.IViewModel;

public interface ISelectShowViewModel
{
    string Title { get; }

    ObservableCollection<ShowOverviewDto> Shows { get; }

    ShowOverviewDto? SelectedShow { get; set; }

    void Initialize();
}
