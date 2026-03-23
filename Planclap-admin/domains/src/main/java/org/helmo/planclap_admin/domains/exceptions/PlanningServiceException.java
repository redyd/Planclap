package org.helmo.planclap_admin.domains.exceptions;

/**
 * Exception lors d'une erreur dans la génération de planning
 */
public class PlanningServiceException extends Exception
{
    public PlanningServiceException(String message) {
        super(message);
    }
}
