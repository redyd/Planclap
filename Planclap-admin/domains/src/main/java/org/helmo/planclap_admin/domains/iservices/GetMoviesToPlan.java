package org.helmo.planclap_admin.domains.iservices;

import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.core.MoviesToPlan;

public interface GetMoviesToPlan {

    /**
     * Récupère les films à planifier
     * @return les films à planifier
     * @throws RepositoryException si une erreur survient lors de la lecture des données
     */
    MoviesToPlan getMoviesToPlan() throws RepositoryException;
}
