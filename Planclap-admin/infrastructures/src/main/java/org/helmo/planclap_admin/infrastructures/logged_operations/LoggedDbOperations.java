package org.helmo.planclap_admin.infrastructures.logged_operations;

import org.helmo.planclap_admin.domains.annotations.Log;
import org.helmo.planclap_admin.domains.exceptions.SqlWrapperException;
import org.helmo.planclap_admin.infrastructures.helper.SqlWrapper;

import java.util.Properties;

public class LoggedDbOperations {

    @Log
    public static SqlWrapper withTransaction(String url, Properties props) throws SqlWrapperException {
        return SqlWrapper.withTransaction(url, props);
    }

    @Log
    public static SqlWrapper withAutoCommit(String url, Properties props) throws SqlWrapperException {
        return SqlWrapper.withAutoCommit(url, props);
    }

}
