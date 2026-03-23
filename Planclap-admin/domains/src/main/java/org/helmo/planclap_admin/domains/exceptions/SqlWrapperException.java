package org.helmo.planclap_admin.domains.exceptions;

public class SqlWrapperException extends RuntimeException {
    public SqlWrapperException(String message, Exception e) {
        super(message, e);
    }
}
