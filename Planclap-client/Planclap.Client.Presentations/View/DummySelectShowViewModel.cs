using System.Collections.ObjectModel;
using Planclap.Client.Presentations.Dtos;
using Planclap.Client.Presentations.IViewModel;

namespace Planclap.Client.Presentations.View;

public class DummySelectShowViewModel : ISelectShowViewModel
{
    private static readonly Uri PosterUri =
        new("https://theposterdb.com/api/assets/468365/view");

    public string Title => "Les films d'aujourd'hui";

    public ObservableCollection<ShowOverviewDto> Shows =>
    [
        new(
            PosterUri,
            "La Nuit des Étoiles",
            new TimeOnly(10, 30),
            TimeSpan.FromMinutes(95),
            false),

        new(
            PosterUri,
            "Les Gardiens du Temps",
            new TimeOnly(13, 15),
            TimeSpan.FromMinutes(128),
            false),

        new(
            PosterUri,
            "Horizons Perdus",
            new TimeOnly(15, 45),
            TimeSpan.FromMinutes(110),
            true),

        new(
            PosterUri,
            "Mission Ultime",
            new TimeOnly(18, 0),
            TimeSpan.FromMinutes(142),
            true),

        new(
            PosterUri,
            "Rêves d’Aurore",
            new TimeOnly(20, 30),
            TimeSpan.FromMinutes(105),
            true)
    ];

    public ShowOverviewDto? SelectedShow { get; set; }

    public void Initialize() => SelectedShow = null;
}
