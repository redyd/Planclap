package org.helmo.planclap_admin.domains.iservices;

import org.helmo.planclap_admin.domains.core.Name;
import org.helmo.planclap_admin.domains.core.MovieSessions;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;

import java.util.Optional;

public interface SearchMovieToPlan {

    /**
     * Recherche un film dans les films à planifier.
     *
     * @param slug le slug du film à rechercher
     * @return un optionnel avec le film à planifier avec son nombre de séances, sinon un optionnel vide
     * @throws RepositoryException si une erreur survient lors de la lecture des données
     */
    Optional<MovieSessions> searchMovieToPlan(Name slug) throws RepositoryException;
}
