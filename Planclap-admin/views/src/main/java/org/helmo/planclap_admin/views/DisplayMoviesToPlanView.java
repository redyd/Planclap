package org.helmo.planclap_admin.views;

import org.helmo.planclap_admin.presentations.iview.DisplayMoviesToPlanPresenterView;
import org.helmo.planclap_admin.presentations.view_models.MovieSessionsViewModel;
import org.helmo.planclap_admin.presentations.view_models.MoviesToPlanViewModel;

import java.io.BufferedReader;
import java.io.PrintStream;

public class DisplayMoviesToPlanView extends AbstractCliView implements DisplayMoviesToPlanPresenterView {

    public DisplayMoviesToPlanView(BufferedReader cin, PrintStream cout) {
        super(cin, cout);
    }

    @Override
    public void displayMoviesToPlan(MoviesToPlanViewModel moviesToPlan) {
        printf("[FILMS A PLANIFIER POUR LE %s]\n", moviesToPlan.date());

        if (!moviesToPlan.movieSessions().isEmpty()) {
            printf("%50s | %10s | %10s |\n", "Titre", "Duree", "Seances");
            printf("-=".repeat((50 + 3 + 10 + 3 + 10 + 2)/2) + "\n");

            for (MovieSessionsViewModel movie : moviesToPlan.movieSessions()) {
                printf("%50s | %10s | %10s |\n", movie.title(), movie.duration(), movie.sessionsAmount());
            }
        } else {
            printf("Aucune seance pour le moment\n");
        }

    }

    @Override
    public void displayTotalDuration(String totalDuration) {
        printf("[TOTAL A PLANIFIER] %s\n", totalDuration);
    }

    @Override
    public void displayErrorMessage(String message) {
        addErrorMessage(message);
    }
}
