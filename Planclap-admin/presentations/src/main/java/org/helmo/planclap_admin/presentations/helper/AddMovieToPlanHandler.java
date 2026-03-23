package org.helmo.planclap_admin.presentations.helper;

import org.helmo.planclap_admin.domains.core.Movie;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.exceptions.TooMuchMoviesException;
import org.helmo.planclap_admin.domains.iservices.AddMovieToPlan;
import org.helmo.planclap_admin.presentations.iview.AddMovieToPlanPresenterView;

import java.util.Optional;

public class AddMovieToPlanHandler {

    private final AddMovieToPlan services;
    private final AddMovieToPlanPresenterView view;

    public AddMovieToPlanHandler(AddMovieToPlan services, AddMovieToPlanPresenterView view) {
        this.services = services;
        this.view = view;
    }

    /**
     * Gère l'exécution de l'ajout d'un film.
     *
     * @param sessions le film à ajouter
     */
    public void execute(MovieSessions sessions) {
        try {
            if (services.canAddMovieToPlan(sessions)) {
                services.addMovieToPlan(sessions);
                view.displaySuccessMessage("Film correctement ajoute !");
            } else {
                view.displayErrorMessage("Impossible d'ajouter ce film a planifier");
            }
        } catch (TooMuchMoviesException e) {
            view.displayErrorMessage("Ajout impossible: trop de film deja planifie");
        } catch (RepositoryException e) {
            view.displayErrorMessage("Une erreur s'est produite lors de l'ajout");
        }
    }

    /**
     * Vérifie si le slug existe déjà.
     *
     * @param slug le slug à vérifier
     * @return {@code true} si le slug existe, sinon {@code false}
     */
    public boolean slugExists(Name slug) {
        try {
            return services.slugExists(slug);
        } catch (RepositoryException e) {
            view.displayErrorMessage("Une erreur s'est produite lors de la verification");
            return true;
        }
    }

    /**
     * Récupère optionnellement un film déjà présent sur base de son slug.
     *
     * @param slug le slug du film à chercher
     * @return un optionnel du film voulu
     */
    public Optional<Movie> getOnlyMovie(Name slug) {
        try {
            return services.isAlreadyDefine(slug);
        } catch (RepositoryException e) {
            view.displayErrorMessage("Une erreur s'est produite lors de la tentative de retrouver le film");
            return Optional.empty();
        }
    }
}