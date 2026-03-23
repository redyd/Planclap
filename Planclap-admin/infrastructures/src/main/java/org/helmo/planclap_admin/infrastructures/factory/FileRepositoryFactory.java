package org.helmo.planclap_admin.infrastructures.factory;

import org.helmo.planclap_admin.domains.repository.MoviesToPlanRepository;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;
import org.helmo.planclap_admin.infrastructures.implementations.CsvPlanningRepository;
import org.helmo.planclap_admin.infrastructures.implementations.JsonMoviesToPlanRepository;

import java.nio.file.Path;

public class FileRepositoryFactory implements DatasourceRepositoryFactory {

    private final Path path;

    public FileRepositoryFactory(Path path) {
        this.path = path;
    }

    @Override
    public MoviesToPlanRepository createMoviesRepository() {
        return new JsonMoviesToPlanRepository(path);
    }

    @Override
    public PlanningRepository createPlanningRepository() {
        return new CsvPlanningRepository(path);
    }

}

