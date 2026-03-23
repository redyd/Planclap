package org.helmo.planclap_admin.domains.exceptions;

/**
 * Exception lorsqu'il y a trop de film à planifier
 */
public class TooMuchMoviesException extends RuntimeException {
    public TooMuchMoviesException(String message) {
        super(message);
    }
}
