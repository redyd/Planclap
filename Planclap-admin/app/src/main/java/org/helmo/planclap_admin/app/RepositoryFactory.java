package org.helmo.planclap_admin.app;

import org.helmo.planclap_admin.domains.exceptions.InvalidArgsException;
import org.helmo.planclap_admin.infrastructures.factory.DatasourceRepositoryFactory;
import org.helmo.planclap_admin.infrastructures.factory.DbRepositoryFactory;
import org.helmo.planclap_admin.infrastructures.factory.FileRepositoryFactory;

public abstract class RepositoryFactory {

    public static DatasourceRepositoryFactory fromArgs(String[] args) {
        var file = new FileNode(null);
        var db = new DbNode(file);
        var prop = db.test(args);

        return switch (prop) {
            case FileNode f -> new FileRepositoryFactory(f.getPath());
            case DbNode d -> new DbRepositoryFactory(d.getDb(), d.getProps());
            default -> throw new InvalidArgsException("Unknown repository option: " + prop);
        };
    }
}

