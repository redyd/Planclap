package org.helmo.planclap_admin.infrastructures.factory;

import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;
import org.helmo.planclap_admin.infrastructures.implementations.SqlMoviesToPlanRepository;
import org.helmo.planclap_admin.infrastructures.implementations.SqlPlanningRepository;

import java.util.Properties;

public class DbRepositoryFactory implements DatasourceRepositoryFactory {

    private final String db;
    private final Properties props;

    public DbRepositoryFactory(String db, Properties props) {
        this.db = db;
        this.props = props;
    }

    @Override
    public MoviesToPlanRepository createMoviesRepository() {
        return new SqlMoviesToPlanRepository(db, props);
    }

    @Override
    public PlanningRepository createPlanningRepository() {
        return new SqlPlanningRepository(db, props);
    }

}
