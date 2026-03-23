package org.helmo.planclap_admin.app;

import joptsimple.OptionParser;
import joptsimple.OptionSet;
import org.helmo.planclap_admin.domains.exceptions.InvalidArgsException;

import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.Properties;

public class DbNode extends ArgsNode {

    private static final String ARG_DB = "db";
    private static final String ARG_USER = "user";
    private static final String ARG_PWD = "pwd";

    private String db;
    private Properties props;

    public DbNode(ArgsNode next) {
        super(next);
    }

    public String getDb() {
        return db;
    }

    public Properties getProps() {
        return props;
    }

    @Override
    public ArgsNode test(String[] args) {
        // create the parser
        OptionParser parser = new OptionParser();
        parser.accepts(ARG_DB).withOptionalArg().ofType(String.class);
        parser.accepts(ARG_USER).withOptionalArg().ofType(String.class);
        parser.accepts(ARG_PWD).withOptionalArg().ofType(String.class);
        parser.allowsUnrecognizedOptions();

        // parse the option
        OptionSet option = parser.parse(args);

        if (!option.has(ARG_DB) || !option.has(ARG_USER) || !option.has(ARG_PWD)) {
            return next(args);
        }

        this.db = (String) option.valueOf(ARG_DB);
        this.props = new Properties();
        props.setProperty("user", (String) option.valueOf(ARG_USER));
        props.setProperty("password", (String) option.valueOf(ARG_PWD));

        return tryConnect();
    }

    private DbNode tryConnect() {
        try (var ignored = DriverManager.getConnection(db, props)){
            return this;
        } catch (SQLException ex) {
            throw new InvalidArgsException("invalid login or password");
        }
    }
}
