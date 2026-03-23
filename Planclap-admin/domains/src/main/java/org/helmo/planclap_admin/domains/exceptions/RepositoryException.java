package org.helmo.planclap_admin.domains.exceptions;

/**
 * Exception lors d'une erreur de repository
 */
public class RepositoryException extends Exception {
    public RepositoryException(String message) {
        super(message);
    }
}
