package org.helmo.planclap_admin.domains.services;

import org.helmo.planclap_admin.domains.core.PlanningGenerator;
import org.helmo.planclap_admin.domains.core.Planning;
import org.helmo.planclap_admin.domains.exceptions.RepositoryException;
import org.helmo.planclap_admin.domains.exceptions.PlanningServiceException;
import org.helmo.planclap_admin.domains.iservices.GeneratePlanning;
import org.helmo.planclap_admin.domains.repository.PlanningRepository;

import java.time.LocalDate;

public class PlanningService extends Services implements GeneratePlanning {

    private final PlanningGenerator generator;
    private final PlanningRepository planningRepository;
    private final MoviesToPlanServices moviesToPlanServices;

    public PlanningService(PlanningGenerator generator, PlanningRepository planningRepository, MoviesToPlanServices moviesToPlanServices) {
        this.generator = generator;
        this.planningRepository = planningRepository;
        this.moviesToPlanServices = moviesToPlanServices;
    }

    @Override
    public void generatePlanning() throws PlanningServiceException, RepositoryException {
        try {
            LocalDate date = getNextMonday();
            Planning planning = generator.generate(moviesToPlanServices.getMoviesToPlan());
            planningRepository.encode(planning, date);
        } catch (IllegalArgumentException e) {
            throw new PlanningServiceException(e.getMessage());
        }
    }
}
