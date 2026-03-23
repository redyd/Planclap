package org.helmo.planclap_admin.domains.iservices;

import java.sql.ResultSet;
import java.sql.SQLException;

@FunctionalInterface
public interface ResultSetMapper<R> {
    R map(ResultSet rs) throws SQLException;
}
