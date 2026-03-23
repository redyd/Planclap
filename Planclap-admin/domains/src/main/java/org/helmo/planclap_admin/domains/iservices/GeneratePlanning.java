package org.helmo.planclap_admin.domains.iservices;

import org.helmo.planclap_admin.domains.exceptions.PlanningServiceException;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;

public interface GeneratePlanning {

    /**
     * Génère un nouveau planning
     * @throws PlanningServiceException si la génération a échoué
     * @throws RepositoryException si l'enregistrement du planning a échoué
     */
    void generatePlanning() throws PlanningServiceException, RepositoryException;
}
