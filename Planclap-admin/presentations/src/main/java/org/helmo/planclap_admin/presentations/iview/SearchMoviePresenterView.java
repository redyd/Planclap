package org.helmo.planclap_admin.presentations.iview;

import org.helmo.planclap_admin.presentations.view_models.MovieSessionsViewModel;

public interface SearchMoviePresenterView {
    String getSlug();

    void displayMovie(MovieSessionsViewModel movie);
    void displayError(String error);
    void notFound();
}
