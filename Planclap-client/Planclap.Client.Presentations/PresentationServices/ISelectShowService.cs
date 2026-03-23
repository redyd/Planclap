using Planclap.Client.Presentations.Dtos;

namespace Planclap.Client.Presentations.PresentationServices;

public interface ISelectShowService
{
    DateTime Current { get; }

    void OnMovieClick(ShowOverviewDto showDetailViewModel);
}
