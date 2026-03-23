package org.helmo.planclap_admin.views;

import org.helmo.planclap_admin.presentations.iview.SearchMoviePresenterView;
import org.helmo.planclap_admin.presentations.view_models.MovieSessionsViewModel;

import java.io.BufferedReader;
import java.io.PrintStream;
import java.util.List;

public class SearchMovieView extends AbstractCliView implements SearchMoviePresenterView {

    public SearchMovieView(BufferedReader cin, PrintStream cout) {
        super(cin, cout);
    }


    @Override
    public String getSlug() {
        return readString("Nom du film: ");
    }

    @Override
    public void displayMovie(MovieSessionsViewModel movie) {
        printf("\n[RESULTAT DE LA RECHERCHE]\n");
        printf("%s\n", movie.title());
        printf("%s\n", movie.description());
        printf("Duree: %s\n", movie.duration());
        printf("Cinechecks: %s\n", convertListToString(movie.cinechecks()));
        printf("URL: %s\n", movie.poster());
        printf("Seances a planifier: %d\n", movie.sessionsAmount());
        printf("%s\n\n", "=".repeat(26));
    }

    private String convertListToString(List<String> movies) {
        StringBuilder sb = new StringBuilder();
        for (String content : movies) {
            sb.append(content);
            sb.append(", ");
        }
        sb.delete(sb.length() - 2, sb.length());
        return sb.toString();
    }

    @Override
    public void notFound() {
        addWarningMessage("Aucune correspondance trouvee");
    }

    @Override
    public void displayError(String error) {
        addErrorMessage(error);
    }
}
