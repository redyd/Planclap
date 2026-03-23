package org.helmo.planclap_admin.infrastructures.factory;

import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;

public interface DatasourceRepositoryFactory {
    MoviesToPlanRepository createMoviesRepository();

    PlanningRepository createPlanningRepository();
}
