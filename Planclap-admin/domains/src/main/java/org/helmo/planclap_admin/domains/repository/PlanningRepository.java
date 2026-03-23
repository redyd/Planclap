package org.helmo.planclap_admin.domains.repository;

import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;

import java.time.LocalDate;

public interface PlanningRepository {

    /**
     * Encode un planning.
     *
     * @param planning le planning à enregistrer
     * @param date la date du planning
     * @throws RepositoryException si une erreur survient lors de l'écriture des données
     */
    void encode(Planning planning, LocalDate date) throws RepositoryException;
}
