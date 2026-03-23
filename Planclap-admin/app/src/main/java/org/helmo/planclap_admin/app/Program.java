package org.helmo.planclap_admin.app;

import org.helmo.planclap_admin.domains.core.RandomPlanningGenerator;
import org.helmo.planclap_admin.domains.exceptions.InvalidArgsException;
import org.helmo.planclap_admin.domains.services.MoviesToPlanServices;
import org.helmo.planclap_admin.domains.services.PlanningService;
import org.helmo.planclap_admin.domains.services.ServicesNotifier;
import org.helmo.planclap_admin.presentations.commands.CommandMap;
import org.helmo.planclap_admin.presentations.presenters.AddMovieToPlanPresenter;
import org.helmo.planclap_admin.presentations.presenters.DisplayMoviesToPlanPresenter;
import org.helmo.planclap_admin.presentations.presenters.GeneratePlanningPresenter;
import org.helmo.planclap_admin.presentations.presenters.SearchMoviePresenter;
import org.helmo.planclap_admin.views.AddMovieToPlanView;
import org.helmo.planclap_admin.views.DisplayMoviesToPlanView;
import org.helmo.planclap_admin.views.GeneratePlanningView;
import org.helmo.planclap_admin.views.SearchMovieView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintStream;

/**
 * Point d'entrée du programme
 */
public class Program {
    public static void main(String[] args) throws IOException {
        try (PrintStream cout = System.out) {
            try (BufferedReader cin = new BufferedReader(new InputStreamReader(System.in))) {
                var factory = RepositoryFactory.fromArgs(args);
                var moviesToPlanRepository = factory.createMoviesRepository();
                var planningRepository = factory.createPlanningRepository();

                // UTILS
                var generator = new RandomPlanningGenerator();

                // SERVICES
                var notifier = new ServicesNotifier();
                var moviesToPlanServices = new MoviesToPlanServices(moviesToPlanRepository, notifier);
                var planningService = new PlanningService(generator, planningRepository, moviesToPlanServices);

                // COMMANDS
                initCommand(cin, cout, moviesToPlanServices, notifier, planningService);
            } catch (InvalidArgsException e) {
                leave(cout);
            }
        }

    }

    private static void leave(PrintStream cout) {
        cout.println("=== argument requis db manquant ou incorrect ===");
    }

    private static void initCommand(BufferedReader cin, PrintStream cout, MoviesToPlanServices moviesToPlanServices, ServicesNotifier notifier, PlanningService planningService) {
        var command = new CommandMap(cin, cout);
        command.add("Films a planifier", DisplayMoviesToPlanPresenter.of(moviesToPlanServices, new DisplayMoviesToPlanView(cin, cout), notifier));
        command.add("Ajouter un film a planifier", AddMovieToPlanPresenter.of(moviesToPlanServices, new AddMovieToPlanView(cin, cout)));
        command.add("Rechercher un film", SearchMoviePresenter.of(moviesToPlanServices, new SearchMovieView(cin, cout)));
        command.add("Generer le planning", GeneratePlanningPresenter.of(planningService, new GeneratePlanningView(cin, cout)));

        notifier.notifySubscribers();
        command.execute();
    }
}
